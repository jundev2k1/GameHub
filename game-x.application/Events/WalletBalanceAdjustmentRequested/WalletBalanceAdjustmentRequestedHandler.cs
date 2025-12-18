using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUserBalanceUpdated;

namespace game_x.application.Events.WalletBalanceAdjustmentRequested;

public sealed class WalletBalanceAdjustmentRequestedHandler(
    IUnitOfWork unitOfWork,
    ITransactionRepo transactionRepo,
    IGameProviderCacheService gameProviderCache,
    IWalletManagerCacheService walletManagerCache,
    IApplicationEventDispatcher eventDispatcher) : IApplicationEventHandler<WalletBalanceAdjustmentRequestedEvent>
{
    public async Task Handle(WalletBalanceAdjustmentRequestedEvent @event, CancellationToken ct = default)
    {
        // Refresh balance from the third party first
        var refreshBalanceEvent = new OnUserBalanceUpdatedEvent(@event.UserId, @event.PlatformId);
        await eventDispatcher.Publish(refreshBalanceEvent, ct);

        var platform = gameProviderCache.PlatformList
            .FirstOrDefault(pl => pl.Id == @event.PlatformId)
            ?? throw new NotFoundException(nameof(@event.PlatformId), @event.PlatformId);
        var latestTransaction = await transactionRepo
            .GetLatestExternalTransactionAsync(@event.UserId, platform.LocalId, ct);
        if (latestTransaction is null) return;

        var userWallet = await walletManagerCache.GetWalletAsync(@event.UserId);
        var platformWallet = userWallet.ExternalWallets
            .FirstOrDefault(w => w.PlatformId == platform.Id)
            ?? throw new NotFoundException(nameof(platform.Id), platform.Id);
        // In case of the current balance remains unchanged, exit this function
        if (latestTransaction.BalanceAfter == platformWallet.Amount)
            return;

        // Write a balance adjustment transaction
        await unitOfWork.WithTransactionAsync(async () =>
        {
            var differenceBalance = platformWallet.Amount - latestTransaction.BalanceAfter ?? 0;
            var transaction = Transaction.Create(
                @event.UserId,
                differenceBalance,
                latestTransaction.CryptoTokenId,
                TransactionSourceType.G598SnoGameProvider,
                TransactionType.BalanceAdjustment);
            transaction.ConfirmTx(platformWallet.Amount, platformWallet.Amount, DateTime.UtcNow);
            await transactionRepo.AddAsync(transaction, ct);
        }, ct);
    }
}
