using game_x.application.Contract.Infrastructure.ExternalApi.PaymentGateway;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnWithdrawalOrderReviewed;
using game_x.application.Features.Transactions.Dtos;
using game_x.application.Features.Transactions.Mapping;
using game_x.share.ExternalApi.PaymentGateway.Dtos;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.Transactions.Admin.Commands.TraceV2.AdminReviewWithdrawalOrderV2;

public sealed class AdminReviewWithdrawalOrderV2Handler(
    IPaymentGatewayService pgService,
    IUnitOfWork unitOfWork,
    ITransactionRepo transactionRepo,
    IApplicationEventDispatcher eventDispatcher,
    IUserBalanceRepo userBalanceRepo,
    IUserAccessor userAccessor,
    IOptions<GameXSettings> gameXSettings,
    IAsymmetricCryptoService asymmetricCryptoService,
    IAppLogger<AdminReviewWithdrawalOrderV2Handler> logger)
    : ICommandHandler<AdminReviewWithdrawalOrderV2Command, ListTransactionInternalDto>
{
    public async Task<ListTransactionInternalDto> Handle(AdminReviewWithdrawalOrderV2Command request, CancellationToken ct = default)
    {
        var tx = await transactionRepo.GetInternalByIdAsync(request.OrderId ?? Guid.Empty, ct);

        if (!tx.Type.Equals(TransactionType.Withdrawal))
            throw new BadRequestException(MessageCode.Transaction.InvalidTradeType);

        if (!tx.Status.Equals(TransactionStatus.Pending))
            throw new BadRequestException(MessageCode.System.InvalidCurrentStatus);

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

        var newTx = await transactionRepo.GetInternalByIdAsync(request.OrderId ?? Guid.Empty, ct);
        await eventDispatcher.Publish(new OnWithdrawalOrderReviewedEvent(newTx.Adapt<TransactionInternalDto>()), ct);

        return newTx.Adapt<ListTransactionInternalDto>();
    }

    private async Task HandleApproveTransactionAsync(Transaction transaction, CancellationToken ct)
    {
        Exception? ex = null;
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await transactionRepo.UpdateAsync(transaction.PublicId, async tx =>
            {
                tx.Review(true, userAccessor.GetUserId());
                ex = await SendPaymentGatewayRequest(tx, ct);
            });
        }, ct);

        if (ex != null) throw ex;
    }

    private async Task HandleRejectTransactionAsync(Transaction transaction, CancellationToken ct)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await transactionRepo.UpdateAsync(transaction.PublicId, async tx =>
            {
                tx.Review(false, userAccessor.GetUserId());
                await TryRefundFrozenBalanceAsync(tx, ct);
            }, ct);
        }, ct);
    }

    private async Task<Exception?> SendPaymentGatewayRequest(Transaction tx, CancellationToken ct)
    {
        try
        {
            var pgRequest = CreatePaymentGatewayRequest(tx, tx.TransactionInternal!.ProviderId);
            var result = await pgService.ProxyWithdrawalAsync(pgRequest);
            var secretKey = gameXSettings.Value.PaymentGatewaySecretKey;
            bool isValid = asymmetricCryptoService.PaymentGatewayVerifySignature(secretKey, result.Data, result.Signature);
            if (!isValid) return new BadRequestException(MessageCode.System.TokenGenerationFailed, "Invalid signature.");

            return null;
        }
        catch (Exception ex)
        {
            await unitOfWork.WithTransactionAsync(async () =>
            {
                await transactionRepo.UpdateAsync(tx.PublicId, async transaction =>
                {
                    transaction.UpdateStatus(TransactionStatus.Failed);
                    transaction.UpdateMeta(m => m.ErrorMessage = ex.Message);
                    await TryRefundFrozenBalanceAsync(tx, ct);
                }, ct);
            }, ct);

            logger.LogError(ex, ex.Message);
            return ex;
        }
    }

    private SecureRequest<WithdrawalOrderRequest> CreatePaymentGatewayRequest(Transaction tx, PaymentGatewayProvider providerId)
    {
        var secretKey = gameXSettings.Value.PaymentGatewaySecretKey;
        var payload = tx.ToPaymentGatewayWithdrawalOrderRequest(providerId: (int)providerId);
        var signature = asymmetricCryptoService.PaymentGatewaySign(secretKey, payload);
        return new SecureRequest<WithdrawalOrderRequest> { Data = payload, Signature = signature };
    }

    private async Task TryRefundFrozenBalanceAsync(Transaction tx, CancellationToken ct)
    {
        await userBalanceRepo.UpdateByTokenIdAsync(tx.UserId, tx.CryptoTokenId, balance =>
        {
            var refundAmount = tx.TotalAmount;
            balance.Unfreeze(refundAmount);
        }, ct);
    }
}
