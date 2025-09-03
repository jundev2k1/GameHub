using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.Services.Wallet;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnWithdrawalOrderReviewed;
using game_x.application.Features.ChainTransactions.Dtos;
using game_x.application.Features.ChainTransactions.Mapping;
using game_x.share.ExternalApi.Uxm.Dtos;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.ChainTransactions.Admin.Commands.AdminReviewWithdrawalOrder;

public sealed class AdminReviewWithdrawalOrderHandler(
    IUxmService uxmService,
    IUnitOfWork unitOfWork,
    IUserBalanceService userBalanceService,
    ITransactionRepo transactionRepo,
    IApplicationEventDispatcher eventDispatcher,
    IUserBalanceRepo userBalanceRepo,
    IAppLogger<Transaction> logger,
    IOptions<GameXSettings> gameXSettings,
    IAsymmetricKeyCacheService asymmetricKeyCacheService,
    IAsymmetricCryptoService asymmetricCryptoService)
    : ICommandHandler<AdminReviewWithdrawalOrderCommand, ListTransactionInternalDto>
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
        
        await eventDispatcher.Publish(new OnWithdrawalOrderReviewedEvent(tx.Adapt<TransactionInternalDto>()), ct);
        
        return tx.Adapt<ListTransactionInternalDto>();
    }

    private async Task HandleApproveTransactionAsync(Transaction transaction, CancellationToken ct)
    {
        transaction.UpdateStatus(TransactionStatus.Approved);
        await transactionRepo.PutUpdateAsync(transaction, ct);
        
        await SendUxmWithdrawalOrderAsync(transaction, ct);
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
    
    private async Task SendUxmWithdrawalOrderAsync(Transaction tx, CancellationToken ct)
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