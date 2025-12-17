using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.GameBaccarat;
using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.Services.Wallet;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUserBalanceUpdated;
using game_x.application.Features.Games.Dtos;
using game_x.application.Features.Games.Services;
using game_x.application.Utils;
using game_x.share.ExternalApi.GameBaccarat.Dtos.Withdrawal;
using game_x.share.ExternalApi.GameProvider.Dtos.Withdrawal;

namespace game_x.application.Features.Games.Client.Commands.GameWallet.Withdrawal;

public sealed class WalletWithdrawalHandler(
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
    IGamePlatformService gamePlatformService,
    IAppLogger<Transaction> logger) : ICommandHandler<WalletWithdrawalCommand, ListTransactionExternalDto>
{
    public async Task<ListTransactionExternalDto> Handle(WalletWithdrawalCommand request, CancellationToken ct = default)
    {
        var currentUser = await GetCurrentUserAsync(ct);
        currentUser = await gamePlatformService.EnsureExternalAccountCreatedAsync(
            currentUser,
            request.PlatformId,
            ct: ct);

        var balance = await GetUserBalanceAsync(currentUser.Id, request, ct);
        var transaction = await CreateTransactionAsync(currentUser.Id, balance.CryptoToken.Id, request, ct);

        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            // Handle actions related to post-transaction success
            userBalanceService.IncreaseAmount(balance, transaction.Amount);
            await userBalanceRepo.PutUpdateAsync(balance, ct);
            await transactionRepo.PatchUpdateAsync(transaction.PublicId, order =>
            {
                order.UpdateStatus(TransactionStatus.Completed);
                order.BalanceAfter = balance.TotalAmount;
                order.CompletedAt = DateTime.UtcNow;
            }, ct);

            // Rollback all processing if the transaction fails at the third party
            if (request.PlatformId == GameConstants.PLATFORM_ID_G598)
            {
                await WithdrawalToProviderWalletAsync(
                    gameProviderAccount: currentUser.UserExtend!.GameProviderAccount,
                    sno: transaction.TransactionExternal!.SerialNumber,
                    amount: transaction.Amount);
            }
            if (request.PlatformId == GameConstants.PLATFORM_ID_GAMEBACCARAT)
            {
                await WithdrawalToBaccaratWalletAsync(
                    gameUserId: currentUser.UserExtend!.GameBaccaratUserId,
                    sno: transaction.TransactionExternal!.SerialNumber,
                    amount: transaction.Amount);
            }
            await unitOfWork.CommitAsync(ct);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync(ct);
            logger.LogError($"Failed to create withdrawal game transaction. SNO: {transaction.TransactionExternal!.SerialNumber}", ex.Message);
            await transactionRepo.PatchUpdateAsync(transaction.PublicId, order =>
            {
                order.UpdateStatus(TransactionStatus.Failed);
                order.UpdateMeta(m => m.ErrorMessage = ex.Message);
            }, ct);
            await unitOfWork.SaveChangesAsync(ct);
            throw;
        }

        // Refresh wallet cache and notify user
        await eventDispatcher.Publish(new OnUserBalanceUpdatedEvent(transaction.UserId, request.PlatformId), ct);

        var result = await transactionRepo.GetExternalByIdAsync(transaction.PublicId, ct);
        return result.Adapt<ListTransactionExternalDto>();
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

    private async Task<UserBalance> GetUserBalanceAsync(string userId, WalletWithdrawalCommand command, CancellationToken ct)
    {
        var token = await cryptoTokenRepo.GetByIdAsync(command.CryptoTokenId, ct);
        if (token.Status != CryptoTokenStatus.Active)
            throw new BadRequestException(MessageCode.Crypto.CryptoTokenUnsupported);

        var userBalance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(userId, token.Id, ct)
            ?? throw new BadRequestException(MessageCode.Accounting.BalanceNotFound);

        return userBalance;
    }

    /// <summary>The transaction is created in advance to record the history of the transaction process</summary>
    private async Task<Transaction> CreateTransactionAsync(string userId, int cryptoTokenId, WalletWithdrawalCommand command, CancellationToken ct)
    {
        var txExternal = TransactionExternal.Create(
            sno: GameProviderUtils.SnoGenerate(),
            gamePlatformId: gameProviderCache.G598Platform.LocalId);

        var tx = Transaction.Create(
            sourceType: TransactionSourceType.G598SnoGameProvider,
            type: TransactionType.Withdrawal,
            userId: userId,
            amount: command.Amount,
            cryptoTokenId: cryptoTokenId,
            note: command.Note);

        tx.AddTxExternal(txExternal);
        await transactionRepo.AddAsync(tx, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return tx;
    }

    private async Task WithdrawalToProviderWalletAsync(string gameProviderAccount, string sno, decimal amount)
    {
        var withdrawalRequest = new GameWithdrawalRequest
        {
            Account = gameProviderAccount,
            Quota = amount,
            Sno = sno
        };

        var result = await gameProvider.WithdrawalWalletAsync(withdrawalRequest);
        if (!result.IsSuccess)
        {
            if (result.ErrorCode == GameMessage.InsufficientBalance)
                throw new BadRequestException(MessageCode.Accounting.InsufficientBalance);

            throw new BadRequestException(MessageCode.Accounting.WithdrawalToProviderWalletFailed);
        }
    }

    private async Task WithdrawalToBaccaratWalletAsync(string gameUserId, string sno, decimal amount)
    {
        var withdrawalRequest = new GameBaccaratWithdrawalRequest
        {
            UserId = gameUserId,
            Amount = amount,
            Sno = sno
        };

        await gameBaccarat.WithdrawalAsync(withdrawalRequest);
    }
}
