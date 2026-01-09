using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnTransactionUpdated;

namespace game_x.application.Features.Transactions.Admin.Commands.CancelTransaction;

public sealed class CancelTransactionHandler(
    IUnitOfWork unitOfWork,
    ITransactionRepo transactionRepo,
    IUserBalanceRepo userBalanceRepo,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<CancelTransactionCommand>
{
    public async Task<Unit> Handle(CancelTransactionCommand request, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await transactionRepo.UpdateAsync(request.TransactionId, async tx =>
            {
                if (!tx.CanCancelTransaction())
                    throw new BadRequestException(MessageCode.Transaction.InvalidTradeStatus);

                // Update status to cancel
                tx.Cancel();

                // Exit if this isn't a Uxm withdrawal transaction
                if (tx.SourceType != TransactionSourceType.Uxm && tx.Type != TransactionType.Withdrawal)
                    return;

                // In case of that is a Uxm withdrawal transaction, Unlock balance
                await userBalanceRepo.UpdateByTokenIdAsync(tx.UserId, tx.CryptoTokenId, balance =>
                {
                    balance.Unfreeze(tx.Amount);
                }, ct);
            }, ct);
        }, ct);

        // Create notification and send it to related users
        var @event = new OnTransactionUpdatedEvent(request.TransactionId);
        await eventDispatcher.Publish(@event, ct);

        return Unit.Value;
    }
}