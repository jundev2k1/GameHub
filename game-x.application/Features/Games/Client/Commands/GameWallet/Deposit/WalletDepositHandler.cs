using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.GameBaccarat;
using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.Services.Wallet;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnGameRegister;
using game_x.application.Events.OnUserBalanceUpdated;
using game_x.application.Features.Games.Dtos;
using game_x.application.Utils;
using game_x.share.Extensions;
using game_x.share.ExternalApi.GameBaccarat.Dtos.Deposit;
using game_x.share.ExternalApi.GameProvider.Dtos.Deposit;

namespace game_x.application.Features.Games.Client.Commands.GameWallet.Deposit;

public sealed class WalletDepositHandler(
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IUserBalanceService userBalanceService,
    IUserBalanceRepo userBalanceRepo,
    ICryptoTokenRepo cryptoTokenRepo,
    ITransactionRepo transactionRepo,
    IUnitOfWork unitOfWork,
    IApplicationEventDispatcher eventDispatcher,
    IGameProviderService gameProvider,
    IGameBaccaratService gameBaccarat,
    IGameProviderCacheService gameProviderCache,
    IAppLogger<Transaction> logger) : ICommandHandler<WalletDepositCommand, ListTransactionExternalDto>
{
    public async Task<ListTransactionExternalDto> Handle(WalletDepositCommand request, CancellationToken ct = default)
    {
        var targetPlatform = gameProviderCache.PlatformList.FirstOrDefault(gp => gp.Id == request.PlatformId)
            ?? throw new NotFoundException(MessageCode.Accounting.PlatformNotExist);

        var currentUser = await GetCurrentUserAsync(ct);
        if (!CheckExistAccount(currentUser.UserExtend, request.PlatformId))
        {
            var gameRegisterEvent = new OnGameRegisterEvent(request.PlatformId, currentUser.Id);
            await eventDispatcher.Publish(gameRegisterEvent, ct);

            // Retry after account created
            currentUser = await userRepo.GetUserByIdAsync(currentUser.Id, ct);
        }

        var balance = await GetUserBalanceAsync(currentUser.Id, request, ct);
        var transaction = await CreateTransactionAsync(
            currentUser.Id,
            balance.CryptoToken.Id,
            request.Amount,
            targetPlatform.LocalId,
            request.Note,
            ct);

        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            // Handle actions related to post-transaction success
            userBalanceService.DecreaseAmount(balance, transaction.Amount);
            await userBalanceRepo.PutUpdateAsync(balance, ct);

            decimal lastedBalanceAfter = await transactionRepo.GetLatestExternalBalanceAfterAsync(
                transaction.UserId,
                targetPlatform.LocalId,
                ct);

            await transactionRepo.PatchUpdateAsync(transaction.PublicId, order =>
            {
                order.UpdateStatus(TransactionStatus.Completed);
                order.BalanceAfter = lastedBalanceAfter - transaction.Amount;
            }, ct);

            // Rollback all processing if the transaction fails at the third party
            if (request.PlatformId == GameConstants.PLATFORM_ID_G598)
            {
                await DepositToProviderWalletAsync(
                    gameProviderAccount: currentUser.UserExtend!.GameProviderAccount,
                    sno: transaction.TransactionExternal!.SerialNumber,
                    amount: transaction.Amount);
            }
            if (request.PlatformId == GameConstants.PLATFORM_ID_GAMEBACCARAT)
            {
                await DepositToBacaratWalletAsync(
                    gameUserId: currentUser.UserExtend!.GameBaccaratUserId,
                    sno: transaction.TransactionExternal!.SerialNumber,
                    amount: transaction.Amount);
            }
            await unitOfWork.CommitAsync(ct);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync(ct);

            logger.LogError($"Failed to create deposit game transaction. SNO: {transaction.TransactionExternal!.SerialNumber}", ex.Message);
            await transactionRepo.PatchUpdateAsync(transaction.PublicId, order =>
            {
                order.UpdateStatus(TransactionStatus.Failed);
                order.UpdateMeta(m => m.ErrorMessage = ex.Message);
            }, ct);

            throw;
        }

        var @event = new OnUserBalanceUpdatedEvent(transaction.UserId, targetPlatform.Id);
        await eventDispatcher.Publish(@event, ct);

        var result = await transactionRepo.GetExternalByIdAsync(transaction.PublicId, ct);
        return result.Adapt<ListTransactionExternalDto>();
    }

    private static bool CheckExistAccount(UserExtend? usrex, Guid gamePlatformId)
    {
        if (usrex is null) return false;

        if ((gamePlatformId == GameConstants.PLATFORM_ID_G598)
            && (usrex.GameProviderAccount.IsNullOrWhiteSpace() || usrex.GameProviderPassword.IsNullOrWhiteSpace()))
            return false;

        if ((gamePlatformId == GameConstants.PLATFORM_ID_GAMEBACCARAT)
            && (usrex.GameBaccaratAccount.IsNullOrWhiteSpace() || usrex.GameBaccaratPassword.IsNullOrWhiteSpace()))
            return false;

        return true;
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
    private async Task<Transaction> CreateTransactionAsync(
        string userId,
        int cryptoTokenId,
        decimal amount,
        int localPlatformId,
        string? note,
        CancellationToken ct)
    {
        var txExternal = TransactionExternal.Create(
            sno: GameProviderUtils.SnoGenerate(),
            gamePlatformId: localPlatformId);

        var tx = Transaction.Create(
            sourceType: TransactionSourceType.G598SnoGameProvider,
            type: TransactionType.Deposit,
            userId: userId,
            amount: amount,
            cryptoTokenId: cryptoTokenId,
            note: note);

        tx.AddTxExternal(txExternal);
        await transactionRepo.AddAsync(tx, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return tx;
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

    private async Task DepositToBacaratWalletAsync(string gameUserId, string sno, decimal amount)
    {
        var request = new GameBaccaratDepositRequest
        {
            UserId = gameUserId,
            Amount = amount,
            Sno = sno,
        };
        await gameBaccarat.DepositAsync(request);
    }
}
