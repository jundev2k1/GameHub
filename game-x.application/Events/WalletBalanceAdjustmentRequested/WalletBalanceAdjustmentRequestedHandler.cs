using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Utils;

namespace game_x.application.Events.WalletBalanceAdjustmentRequested;

public sealed class WalletBalanceAdjustmentRequestedHandler(
    IUnitOfWork unitOfWork,
    IUserBalanceRepo userBalanceRepo,
    ITransactionRepo transactionRepo,
    IGameProviderCacheService gameProviderCache,
    IWalletManagerCacheService walletManagerCache) : IApplicationEventHandler<WalletBalanceAdjustmentRequestedEvent>
{
    public async Task Handle(WalletBalanceAdjustmentRequestedEvent @event, CancellationToken ct = default)
    {
        // Refresh balance from the third party first
        await walletManagerCache.RefreshExternalWalletAsync(@event.UserId, @event.PlatformId);
        var isExist = await walletManagerCache.IsExistExternalWalletAsync(@event.UserId, @event.PlatformId);
        if (!isExist) throw new BadRequestException(MessageCode.System.DependencyFailure);

        var platform = gameProviderCache.PlatformList
            .FirstOrDefault(pl => pl.Id == @event.PlatformId)
            ?? throw new NotFoundException(nameof(@event.PlatformId), @event.PlatformId);
        var latestTransaction = await transactionRepo
            .GetLatestExternalTransactionAsync(@event.UserId, platform.LocalId, ct);
        if (latestTransaction is null) return;

        var platformWallet = await walletManagerCache.GetExternalWalletAsync(
            @event.UserId,
            platform.Id);
        // In the case of the current balance remains unchanged, exit this function
        if (latestTransaction.GameBalanceAfter == platformWallet.Amount)
            return;

        // Write a balance adjustment transaction
        var differenceBalance = platformWallet.Amount - (latestTransaction.GameBalanceAfter ?? 0);
        await unitOfWork.WithTransactionAsync(async () =>
        {
            var currentBalances = await userBalanceRepo.GetBalancesByUserIdAsync(@event.UserId, ct);
            var internalBalance = currentBalances.Sum(ub => ub.Amount);

            var transaction = Transaction.Create(
                @event.UserId,
                differenceBalance,
                latestTransaction.CryptoTokenId,
                GetTxSourceType(platform.Id),
                TransactionType.BalanceAdjustment);

            var sno = GameProviderUtils.SnoGenerate();
            var externalTx = TransactionExternal.Create(sno, platform.LocalId);
            transaction.AddTxExternal(externalTx);

            transaction.ConfirmGameTx(differenceBalance, internalBalance, platformWallet.Amount);

            await transactionRepo.AddAsync(transaction, ct);
        }, ct);
    }

    private static TransactionSourceType GetTxSourceType(Guid platformId)
    {
        if (platformId == GameConstants.PLATFORM_ID_G598)
            return TransactionSourceType.G598SnoGameProvider;
        if (platformId == GameConstants.PLATFORM_ID_GAMEBACCARAT)
            return TransactionSourceType.BaccaratGameProvider;
        if (platformId == GameConstants.PLATFORM_ID_ETL998_GAMEBACCARAT)
            return TransactionSourceType.Elt998GameProvider;

        throw new NotSupportedException($"This platform ({platformId}) is not support.");
    }
}
