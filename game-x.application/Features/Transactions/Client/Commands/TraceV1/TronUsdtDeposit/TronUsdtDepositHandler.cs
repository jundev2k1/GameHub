using game_x.application.Contract.Infrastructure.ExternalApi.FastPay;
using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Transactions.Dtos;
using game_x.application.Utils;
using game_x.share.Extensions;

namespace game_x.application.Features.Transactions.Client.Commands.TraceV1.TronUsdtDeposit;

public sealed class CreateDepositChainTransactionHandler(
    IUxmService uxmService,
    IFastPayService fastPayService,
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    ICryptoTokenRepo cryptoTokenRepo,
    ITransactionRepo transactionRepo) : ICommandHandler<TronUsdtDepositCommand, DepositChainTransactionResponseDto>
{
    public async Task<DepositChainTransactionResponseDto> Handle(TronUsdtDepositCommand request, CancellationToken ct)
    {
        // Local variables
        var amount = 0M;
        var to = string.Empty;
        var orderUid = string.Empty;
        var txId = Guid.Empty;

        // Handle depositing and updating GameX transaction
        await unitOfWork.WithTransactionAsync(async () =>
        {
            var userId = userAccessor.GetUserId();
            var tx = await CreateTransactionAsync(request, userId, ct);
            txId = tx.PublicId;

            (amount, to, orderUid) = await HandleDepositAsync(request.Provider, userId, tx);
            tx.UpdateProviderResponse(
                null,
                amount: amount,
                providerOrderId: orderUid,
                to: to);
        }, ct);

        // Return new transaction state
        var updatedTransaction = await transactionRepo.GetInternalByIdAsync(txId, ct);
        return new DepositChainTransactionResponseDto
        {
            Amount = amount,
            To = to,
            Transaction = updatedTransaction.Adapt<ListTransactionInternalDto>(),
        };
    }

    private async Task<Transaction> CreateTransactionAsync(
        TronUsdtDepositCommand request,
        string userId,
        CancellationToken ct)
    {
        var token = await cryptoTokenRepo.GetByIdAsync(request.CryptoTokenId, ct);
        if (token.Status != CryptoTokenStatus.Active)
            throw new BadRequestException(MessageCode.Crypto.CryptoTokenUnsupported);

        var txInternal = TransactionInternal.Create(
            orderNumber: OrderNoGenerator.Otc(),
            providerId: request.Provider,
            sourceType: TransactionSourceType.Payment);

        var tx = Transaction.Create(
            type: TransactionType.Deposit,
            userId: userId,
            amount: request.Amount,
            cryptoTokenId: token.Id,
            note: request.Note);

        tx.AddTxInternal(txInternal);

        await transactionRepo.AddAsync(tx, ct);
        return tx;
    }

    private async Task<(decimal Amount, string To, string OrderUid)> HandleDepositAsync(
        PaymentGatewayProvider provider,
        string userId,
        Transaction transaction)
    {
        switch (provider)
        {
            case PaymentGatewayProvider.Uxm:
                var uxmRes = await uxmService.DepositAsync(
                    transaction.Amount,
                    (transaction.TransactionInternal?.OrderNumber).ToStringOrEmpty(),
                    userId,
                    transaction.Note.ToStringOrEmpty());
                return (uxmRes.Amount, uxmRes.To, uxmRes.OrderUid);

            case PaymentGatewayProvider.FastPay:
                var fastPayRes = await fastPayService.DepositAsync(
                    transaction.Amount,
                    (transaction.TransactionInternal?.OrderNumber).ToStringOrEmpty(),
                    userId,
                    transaction.Note.ToStringOrEmpty());
                return (fastPayRes.Amount, fastPayRes.To, fastPayRes.OrderUid);

            default:
                throw new NotSupportedException();
        }
    }
}
