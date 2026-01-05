using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnTransactionInternalCreated;
using game_x.application.Features.Transactions.Dtos;
using game_x.domain.Exceptions;

namespace game_x.application.Features.Transactions.Admin.Commands.CreateTransaction;

public sealed class CreateTransactionHandler(
    IUnitOfWork unitOfWork,
    IUserBalanceRepo userBalanceRepo,
    ITransactionRepo transactionRepo,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<CreateTransactionCommand>
{
    public async Task<Unit> Handle(CreateTransactionCommand request, CancellationToken ct = default)
    {
        var userBalance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(request.UserId, request.CryptoTokenId, ct);
        if (userBalance.Amount + request.Amount < 0)
            throw new InsufficientBalanceException(userBalance.Amount, request.Amount);

        Transaction? transaction = null;
        await unitOfWork.WithTransactionAsync(async () =>
        {
            var balanceAfter = 0M;
            await userBalanceRepo.UpdateByTokenIdAsync(request.UserId, userBalance.CryptoTokenId, balance =>
            {
                balance.AdjustAmount(request.Amount, true);
                balanceAfter = balance.TotalAmount;
            }, ct);

            var internalTx = TransactionInternal.Create(request.Sno);
            transaction = Transaction.Create(
                request.UserId,
                request.Amount,
                userBalance.CryptoTokenId,
                TransactionSourceType.Uxm,
                request.Type,
                note: request.Message);
            transaction.AddTxInternal(internalTx);
            transaction.Confirm(request.Amount, balanceAfter);
            await transactionRepo.AddAsync(transaction, ct);
        }, ct);

        var @event = new OnTransactionInternalCreatedEvent(transaction.Adapt<TransactionInternalDto>());
        await eventDispatcher.Publish(@event, ct);

        return Unit.Value;
    }
}
