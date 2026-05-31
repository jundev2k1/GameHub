using game_x.application.Contract.Infrastructure.ExternalApi.FastPay;
using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.Transactions.OnWithdrawalOrderReviewed;
using game_x.application.Features.Transactions.Dtos;
using game_x.share.Extensions;

namespace game_x.application.Features.Transactions.Admin.Commands.AdminReviewWithdrawalOrder;

public sealed class AdminReviewWithdrawalOrderHandler(
    IFastPayService fastPayService,
    IUxmService uxmService,
    IUnitOfWork unitOfWork,
    ITransactionRepo transactionRepo,
    IApplicationEventDispatcher eventDispatcher,
    IUserBalanceRepo userBalanceRepo,
    IUserAccessor userAccessor) : ICommandHandler<AdminReviewWithdrawalOrderCommand, ListTransactionInternalDto>
{
    public async Task<ListTransactionInternalDto> Handle(AdminReviewWithdrawalOrderCommand request, CancellationToken ct = default)
    {
        var tx = await transactionRepo.GetInternalByIdAsync(request.OrderId ?? Guid.Empty, ct);

        if (!tx.Type.Equals(TransactionType.Withdrawal))
            throw new BadRequestException(MessageCode.Transaction.InvalidTradeType);

        if (!tx.Status.Equals(TransactionStatus.Pending))
            throw new BadRequestException(MessageCode.System.InvalidCurrentStatus);

        if (tx.TransactionInternal is null)
            throw new BadRequestException(MessageCode.System.InvalidResourceState);

        switch (request.OrderStatus)
        {
            case TransactionStatus.Approved:
                await HandleApproveTransactionAsync(tx, ct);
                break;

            case TransactionStatus.Rejected:
                await HandleRejectTransactionAsync(tx, ct);
                break;

            default:
                throw new BadRequestException(MessageCode.System.InvalidParameters);
        }

        // Retrieve target transaction with the latest information
        var newTx = await transactionRepo.GetInternalByIdAsync(tx.PublicId, ct);
        var txNotification = newTx.Adapt<TransactionInternalDto>();

        var orderReviewedEvent = new OnWithdrawalOrderReviewedEvent(txNotification);
        await eventDispatcher.Publish(orderReviewedEvent, ct);

        return newTx.Adapt<ListTransactionInternalDto>();
    }

    private async Task HandleApproveTransactionAsync(Transaction transaction, CancellationToken ct)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await transactionRepo.UpdateAsync(transaction.PublicId, async tx =>
            {
                // Mark the transaction as approved
                tx.Review(true, userAccessor.GetUserId());

                // Handling withdrawals from external systems
                await HandleWithdrawalAsync(tx, ct);
            }, ct);
        }, ct);
    }

    private async Task HandleRejectTransactionAsync(Transaction transaction, CancellationToken ct)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await transactionRepo.UpdateAsync(transaction.PublicId, async tx =>
            {
                // Mark as rejected
                tx.Review(false, userAccessor.GetUserId());

                // Handle unlock the amount of transaction
                await userBalanceRepo.UpdateByTokenIdAsync(tx.UserId, tx.CryptoTokenId, balance =>
                {
                    balance.Unfreeze(tx.TotalAmount);
                }, ct);
            }, ct);
        }, ct);
    }

    private async Task HandleWithdrawalAsync(Transaction tx, CancellationToken ct)
    {
        switch (tx.TransactionInternal!.ProviderId)
        {
            case PaymentGatewayProvider.Uxm:
                // Handle with UXM request
                await uxmService.WithdrawalAsync(
                    tx.Amount,
                    (tx.TransactionInternal?.OrderNumber).ToStringOrEmpty(),
                    (tx.TransactionInternal?.ToAddress).ToStringOrEmpty(),
                    tx.Note.ToStringOrEmpty());
                break;

            case PaymentGatewayProvider.FastPay:
                // Handle with Fast Pay request
                await fastPayService.WithdrawalAsync(
                    tx.Amount,
                    (tx.TransactionInternal?.OrderNumber).ToStringOrEmpty(),
                    (tx.TransactionInternal?.ToAddress).ToStringOrEmpty(),
                    tx.Note.ToStringOrEmpty());
                break;

            default:
                break;
        }
    }
}
