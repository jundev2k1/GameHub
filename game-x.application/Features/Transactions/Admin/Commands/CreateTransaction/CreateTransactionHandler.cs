using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.Transactions.OnTransactionInternalCreated;
using game_x.application.Features.Transactions.Dtos;
using game_x.application.Utils;
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
        var isWithdrawal = request.Type is TransactionTypeRequest.Withdrawal;
        if (isWithdrawal && (userBalance.Amount - request.Amount < 0))
            throw new InsufficientBalanceException(userBalance.Amount, request.Amount);

        Transaction? transaction = null;
        await unitOfWork.WithTransactionAsync(async () =>
        {
            switch (request.Type)
            {
                case TransactionTypeRequest.Deposit:
                case TransactionTypeRequest.Withdrawal:
                    transaction = await HandleUxmTransactionAsync(request, userBalance.CryptoTokenId, ct);
                    break;

                case TransactionTypeRequest.FastPayDeposit:
                    transaction = await HandleFastPayTransactionAsync(request, userBalance.CryptoTokenId, ct);
                    break;
            }
        }, ct);

        var @event = new OnTransactionInternalCreatedEvent(transaction.Adapt<TransactionInternalDto>());
        await eventDispatcher.Publish(@event, ct);

        return Unit.Value;
    }

    private async Task<Transaction> HandleUxmTransactionAsync(CreateTransactionCommand request, int cryptoTokenId, CancellationToken ct)
    {
        var balanceAfter = 0M;
        await userBalanceRepo.UpdateByTokenIdAsync(request.UserId, cryptoTokenId, balance =>
        {
            balance.AdjustAmount(request.Amount, request.Type == TransactionTypeRequest.Deposit);
            balanceAfter = balance.TotalAmount;
        }, ct);

        var internalTx = TransactionInternal.Create(
            orderNumber: OrderNoGenerator.Otc(),
            providerOrderId: request.OrderUId,
            providerId: PaymentGatewayProvider.Uxm,
            sourceType: TransactionSourceType.Refund);
        var transaction = Transaction.Create(
            userId: request.UserId,
            amount: request.Amount,
            cryptoTokenId: cryptoTokenId,
            type: GetTransactionType(request.Type),
            note: request.Message.Trim());
        transaction.AddTxInternal(internalTx);
        transaction.Confirm(request.Amount, balanceAfter);
        await transactionRepo.AddAsync(transaction, ct);
        return transaction;
    }

    private async Task<Transaction> HandleFastPayTransactionAsync(CreateTransactionCommand request, int cryptoTokenId, CancellationToken ct)
    {
        var internalTx = TransactionInternal.Create(
            orderNumber: OrderNoGenerator.Otc(),
            providerOrderId: null,
            providerId: PaymentGatewayProvider.FastPay,
            sourceType: TransactionSourceType.Refund);
        var transaction = Transaction.Create(
            userId: request.UserId,
            amount: request.Amount,
            cryptoTokenId: cryptoTokenId,
            type: GetTransactionType(request.Type),
            note: request.Message.Trim());
        transaction.AddTxInternal(internalTx);
        await transactionRepo.AddAsync(transaction, ct);
        return transaction;
    }

    private static TransactionType GetTransactionType(TransactionTypeRequest type) => type switch
    {
        TransactionTypeRequest.Deposit or TransactionTypeRequest.FastPayDeposit => TransactionType.Deposit,
        TransactionTypeRequest.Withdrawal => TransactionType.Withdrawal,
        _ => throw new BadRequestException(),
    };
}
