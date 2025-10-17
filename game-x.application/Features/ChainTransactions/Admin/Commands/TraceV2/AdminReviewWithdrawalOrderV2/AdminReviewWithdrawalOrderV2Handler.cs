using game_x.application.Contract.Infrastructure.ExternalApi.PaymentGateway;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.Services.Wallet;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnWithdrawalOrderReviewed;
using game_x.application.Features.ChainTransactions.Dtos;
using game_x.application.Features.ChainTransactions.Mapping;
using game_x.share.ExternalApi.PaymentGateway.Dtos;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.ChainTransactions.Admin.Commands.TraceV2.AdminReviewWithdrawalOrderV2;

public sealed class AdminReviewWithdrawalOrderV2Handler(
    IPaymentGatewayService pgService,
    IUnitOfWork unitOfWork,
    IUserBalanceService userBalanceService,
    ITransactionRepo transactionRepo,
    IApplicationEventDispatcher eventDispatcher,
    IUserBalanceRepo userBalanceRepo,
    IAppLogger<Transaction> logger,
    IOptions<GameXSettings> gameXSettings,
    IAsymmetricCryptoService asymmetricCryptoService)
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
        
        await eventDispatcher.Publish(new OnWithdrawalOrderReviewedEvent(tx.Adapt<TransactionInternalDto>()), ct);
        
        return tx.Adapt<ListTransactionInternalDto>();
    }

    private async Task HandleApproveTransactionAsync(Transaction transaction, CancellationToken ct)
    {
        transaction.UpdateStatus(TransactionStatus.Approved);
        await transactionRepo.PutUpdateAsync(transaction, ct);
        await SendPaymentGatewayRequest(transaction, ct);
    }
    
    private async Task HandleRejectTransactionAsync(Transaction transaction, CancellationToken ct)
    {
        transaction.UpdateStatus(TransactionStatus.Rejected);
        
        await unitOfWork.WithTransactionAsync(
            async () =>
            {
                await transactionRepo.PutUpdateAsync(transaction, ct);
                await TryRefundFrozenBalanceAsync(transaction, ct);
            }, ct);
    }

    private async Task SendPaymentGatewayRequest(Transaction tx, CancellationToken ct)
    {
        try
        {
            var pgRequest = CreatePaymentGatewayRequest(tx, tx.TransactionInternal!.ProviderId);
            var result = await pgService.ProxyWithdrawalAsync(pgRequest);
            var apiKey = gameXSettings.Value.PaymentGatewayApiKey;
            bool isValid = asymmetricCryptoService.PaymentGatewayVerifySignature(apiKey, result.Data, result.Signature);
            if (!isValid) throw new BadRequestException(MessageCode.System.TokenGenerationFailed, "Invalid signature.");
        }
        catch (Exception ex)
        {
            await transactionRepo.PatchUpdateAsync(tx.PublicId, x =>
            {
                x.Status = TransactionStatus.Failed;
                x.UpdateMeta(m => m.ErrorMessage = ex.Message);
            }, ct);
            await TryRefundFrozenBalanceAsync(tx, ct);
            throw;
        }
    }
    
    private SecureRequest<WithdrawalOrderRequest> CreatePaymentGatewayRequest(Transaction tx, PaymentGatewayProvider providerId)
    {
        var merchantNumber = gameXSettings.Value.MerchantNumber;
        var platformId = gameXSettings.Value.PaymentGatewayPlatformId;
        var apiKey = gameXSettings.Value.PaymentGatewayApiKey;
        var payload = tx.ToPaymentGatewayWithdrawalOrderRequest(
            merchantNumber: merchantNumber,
            platformId: platformId,
            providerId: (int)providerId);
        var signature = asymmetricCryptoService.PaymentGatewaySign(apiKey, payload);
        return new SecureRequest<WithdrawalOrderRequest>
        {
            Data = payload,
            Signature = signature
        };
    }
    
    private async Task TryRefundFrozenBalanceAsync(Transaction tx, CancellationToken ct)
    {
        UserBalance? balance = tx.User.UserBalances.FirstOrDefault(b => b.CryptoTokenId == tx.CryptoTokenId);
        if (balance == null)
            throw new BadRequestException(MessageCode.Accounting.BalanceNotFound);
        decimal refundAmount = tx.TotalAmount;
        try
        {
            userBalanceService.Unfreeze(balance, refundAmount);
            await userBalanceRepo.PutUpdateAsync(balance, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(
                "[TronWithdrawal] ❌ Balance compensation failed，UserId={UserId}, TokenId={TokenId}, OrderNo={OrderNo}, Refund={RefundAmount}, Err={ex}",
                tx.UserId, 
                tx.CryptoTokenId, 
                tx.TransactionInternal?.OrderNumber ?? string.Empty, 
                refundAmount,
                ex);
            throw new BadRequestException(MessageCode.Accounting.InsufficientFrozenBalance);
        }
    }
}