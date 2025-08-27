using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.Services.UserUsdtLedger;
using game_x.application.Contract.Infrastructure.Services.Wallet;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUserBalanceUpdated;
using game_x.application.Features.Games.Dtos;
using game_x.application.Utils;
using game_x.share.ExternalApi.GameProvider.Dtos.Deposit;

namespace game_x.application.Features.Games.Client.Commands.GameWallet.Deposit;

public sealed class WalletDepositHandler(
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IUserBalanceService userBalanceService,
    IUserBalanceRepo userBalanceRepo,
    ICryptoTokenRepo cryptoTokenRepo,
    IGameTransactionRepo gameTransactionRepo,
    IUserUsdtLedgerService userUsdtLedgerService,
    IUnitOfWork unitOfWork,
    IApplicationEventDispatcher eventDispatcher,
    IGameProviderService gameProvider,
    IGameProviderCacheService gameProviderCache,
    IAppLogger<GameTransaction> logger) : ICommandHandler<WalletDepositCommand, GameTransactionDto>
{
    public async Task<GameTransactionDto> Handle(WalletDepositCommand command, CancellationToken ct = default)
    {
        var currentUser = await GetCurrentUserAsync(ct);
        var balance = await GetUserBalanceAsync(currentUser.Id, command, ct);
        var transaction = await CreateTransactionAsync(currentUser.Id, balance.CryptoToken.Id, command, ct);

        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            // Handle actions related to post-transaction success
            userBalanceService.DecreaseAmount(balance, transaction.Amount);
            await userBalanceRepo.PutUpdateAsync(balance, ct);
            await userUsdtLedgerService.CreateForGameTransactionAsync(transaction);
            await gameTransactionRepo.PatchUpdateAsync(transaction.PublicId, x => x.Status = GameTransactionStatus.Completed, ct);

            // Rollback all processing if the transaction fails at the third party
            await DepositToProviderWalletAsync(
                gameProviderAccount: currentUser.UserExtend!.GameProviderAccount,
                sno: transaction.G598Sno,
                amount: transaction.Amount);

            await eventDispatcher.Publish(new OnUserBalanceUpdatedEvent(transaction.UserId), ct);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync(ct);
            logger.LogError($"Failed to create deposit game transaction. SNO: {transaction.G598Sno}", ex.Message);
            await gameTransactionRepo.PatchUpdateAsync(transaction.PublicId, gt =>
            {
                gt.Status = GameTransactionStatus.Failed;
                gt.UpdateMeta(m => m.ErrorMessage = ex.Message);
            }, ct);

            throw;
        }
        var result = await gameTransactionRepo.GetByIdAsync(transaction.PublicId, ct);
        return result.Adapt<GameTransactionDto>();
    }

    private async Task<User> GetCurrentUserAsync(CancellationToken ct)
    {
        var userId = userAccessor.GetUserId();
        var currentUser = await userRepo.GetUserByIdAsync(userId, ct);
        if (currentUser.UserExtend is null)
            throw new NotFoundException(MessageCode.User.UserExtendNotFound);

        if (!currentUser.EmailConfirmed)
            throw new BadRequestException(MessageCode.User.UserNotConfirmed);

        return currentUser;
    }

    private async Task<UserBalance> GetUserBalanceAsync(string userId, WalletDepositCommand command, CancellationToken ct)
    {
        var token = await cryptoTokenRepo.GetByIdAsync(command.CryptoTokenId, ct);
        if (token.Status != CryptoTokenStatus.Active)
            throw new BadRequestException(MessageCode.Crypto.CryptoTokenUnsupported);

        var userBalance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(userId, token.Id, ct)
            ?? throw new BadRequestException(MessageCode.Accounting.BalanceNotFound);

        if (userBalance.Amount < command.Amount)
            throw new BadRequestException(MessageCode.Accounting.InsufficientBalance);

        return userBalance;
    }

    /// <summary>The transaction is created in advance to record the history of the transaction process</summary>
    private async Task<GameTransaction> CreateTransactionAsync(string userId, int cryptoTokenId, WalletDepositCommand command, CancellationToken ct)
    {
        var sno = GameProviderUtils.SnoGenerate();
        var transaction = GameTransaction.Create(
            userId: userId,
            g598sno: sno,
            amount: command.Amount,
            type: GameTransactionType.Deposit,
            gamePlatformId: gameProviderCache.G598Platform.LocalId,
            cryptoTokenId: cryptoTokenId,
            note: command.Note);

        await gameTransactionRepo.AddAsync(transaction, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return transaction;
    }

    private async Task DepositToProviderWalletAsync(string gameProviderAccount, string sno, decimal amount)
    {
        var depositRequest = new GameDepositRequest
        {
            Account = gameProviderAccount,
            Quota = amount,
            Sno = sno
        };

        var result = await gameProvider.DepositWalletAsync(depositRequest);
        if (!result.IsSuccess)
            throw new BadRequestException(MessageCode.Accounting.DepositToProviderWalletFailed);
    }
}
