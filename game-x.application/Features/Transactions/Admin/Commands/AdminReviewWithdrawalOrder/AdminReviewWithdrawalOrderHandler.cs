using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.Transaction.OnWithdrawalOrderReviewed;
using game_x.application.Features.Transactions.Dtos;
using game_x.application.Features.Transactions.Mapping;
using game_x.share.ExternalApi.Uxm.Dtos;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.Transactions.Admin.Commands.AdminReviewWithdrawalOrder;

public sealed class AdminReviewWithdrawalOrderHandler(
    IUxmService uxmService,
    IUnitOfWork unitOfWork,
    ITransactionRepo transactionRepo,
    IApplicationEventDispatcher eventDispatcher,
    IUserBalanceRepo userBalanceRepo,
    IUserAccessor userAccessor,
    IAppLogger<AdminReviewWithdrawalOrderHandler> logger,
    IOptions<GameXSettings> gameXSettings,
    IAsymmetricKeyCacheService asymmetricKeyCacheService,
    IAsymmetricCryptoService asymmetricCryptoService) : ICommandHandler<AdminReviewWithdrawalOrderCommand, ListTransactionInternalDto>
{
    public async Task<ListTransactionInternalDto> Handle(AdminReviewWithdrawalOrderCommand request, CancellationToken ct = default)
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

        var newTx = await transactionRepo.GetInternalByIdAsync(tx.PublicId, ct);
        var txNotification = newTx.Adapt<TransactionInternalDto>();
        await eventDispatcher.Publish(new OnWithdrawalOrderReviewedEvent(txNotification), ct);

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
                ex = await SendUxmWithdrawalOrderAsync(tx, ct);
            }, ct);
        }, ct);

        if (ex != null) throw ex;
    }

    private async Task HandleRejectTransactionAsync(Transaction transaction, CancellationToken ct)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await transactionRepo.UpdateAsync(transaction.PublicId, tx =>
            {
                tx.Review(false, userAccessor.GetUserId());
            }, ct);
            await TryRefundFrozenBalanceAsync(transaction, ct);
        }, ct);
    }

    private async Task<Exception?> SendUxmWithdrawalOrderAsync(Transaction tx, CancellationToken ct)
    {
        try
        {
            var gameXPrivateKey = asymmetricKeyCacheService.GameXPrivateKey;
            var merchantNumber = gameXSettings.Value.MerchantNumber;

            // Create UXM request data
            var requestData = tx.ToUxmWithdrawalOrderRequest(merchantNumber);
            var uxmRequest = new SecureRequest<UxmWithdrawalOrderRequest>
            {
                Data = requestData,
                Signature = asymmetricCryptoService.Sign(gameXPrivateKey, requestData)
            };
            var result = await uxmService.CreateWithdrawalOrderAsync(uxmRequest);

            // Verify UXM signature
            var uxmPublicKey = asymmetricKeyCacheService.UxmPublicKey;
            var isValid = asymmetricCryptoService.VerifySignature(uxmPublicKey, result.Data, result.Signature);
            if (!isValid)
                throw new BadRequestException(MessageCode.System.TokenGenerationFailed, "Invalid signature.");

            return null;
        }
        catch (Exception ex)
        {
            await unitOfWork.WithTransactionAsync(async () =>
            {
                await transactionRepo.UpdateAsync(tx.PublicId, transaction =>
                {
                    transaction.UpdateStatus(TransactionStatus.Failed);
                    transaction.UpdateMeta(m => m.ErrorMessage = ex.Message);
                }, ct);
                await TryRefundFrozenBalanceAsync(tx, ct);
            }, ct);

            logger.LogError(ex, ex.Message);
            return ex;
        }
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
