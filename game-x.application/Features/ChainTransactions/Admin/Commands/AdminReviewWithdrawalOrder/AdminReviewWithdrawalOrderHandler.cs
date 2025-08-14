using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.Services.Wallet;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnWithdrawalOrderReviewed;
using game_x.application.Features.ChainTransactions.Mapping;
using game_x.share.Extensions;
using game_x.share.ExternalApi.Uxm.Dtos;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.ChainTransactions.Admin.Commands.AdminReviewWithdrawalOrder;

public sealed class AdminReviewWithdrawalOrderHandler(
    IUxmService uxmService,
    IUnitOfWork unitOfWork,
    IUserBalanceService userBalanceService,
    IChainTransactionRepo chainTransactionRepo,
    IApplicationEventDispatcher eventDispatcher,
    IUserBalanceRepo userBalanceRepo,
    IAppLogger<ChainTransaction> logger,
    IOptions<GameXSettings> galaxySettings,
    IAsymmetricKeyCacheService asymmetricKeyCacheService,
    IAsymmetricCryptoService asymmetricCryptoService)
    : ICommandHandler<AdminReviewWithdrawalOrderCommand>
{
    public async Task<Unit> Handle(AdminReviewWithdrawalOrderCommand request, CancellationToken ct = default)
    {
        var transaction = await chainTransactionRepo.GetByIdAsync(request.OrderId, ct);

        // The transaction already exists on the blockchain
        if (transaction.Hash?.IsNotNullOrEmpty() == true)
        {
            logger.LogWarning("Skip duplicate transaction Hash: {TxHash}, UserId: {UserId}, TokenId: {TokenId}", 
                transaction.Hash, transaction?.UserId ?? String.Empty, transaction?.CryptoTokenId.ToString() ?? String.Empty);
            return Unit.Value;
        }
        
        if (!transaction.Status.Equals(ChainTransactionStatus.Pending))
            throw new BadRequestException(MessageCode.System.InvalidCurrentStatus);
        
        switch (request.OrderStatus)
        {
            case ChainTransactionStatus.Approved:
                await HandleApproveTransactionAsync(transaction, ct);
                break;
            case ChainTransactionStatus.Rejected:
                await HandleRejectTransactionAsync(transaction, ct);
                break;
            default:
                throw new BadRequestException(MessageCode.System.InvalidParameters);
        }
        
        await eventDispatcher.Publish(new OnWithdrawalOrderReviewedEvent(transaction), ct);
        return Unit.Value;
    }

    private async Task HandleApproveTransactionAsync(ChainTransaction transaction, CancellationToken ct)
    {
        await chainTransactionRepo
            .PatchUpdateAsync(transaction.PublicId, updatedOrder => 
                updatedOrder.UpdateStatus(ChainTransactionStatus.Approved), ct);
        
        await SendUxmWithdrawalOrderAsync(transaction, ct);
    }
    
    private async Task HandleRejectTransactionAsync(ChainTransaction transaction, CancellationToken ct)
    {
        await unitOfWork.WithTransactionAsync(
            async () =>
            {
                await chainTransactionRepo
                    .PatchUpdateAsync(transaction.PublicId, updatedOrder => 
                        updatedOrder.UpdateStatus(ChainTransactionStatus.Rejected), ct);

                await TryRefundFrozenBalanceAsync(transaction, ct);
            }, ct);
    }
    
    private async Task SendUxmWithdrawalOrderAsync(ChainTransaction chainTransaction, CancellationToken ct)
    {     
        try
        {
            var gameXPrivateKey = asymmetricKeyCacheService.GameXPrivateKey;
            var merchantNumber = galaxySettings.Value.MerchantNumber;
        
            // Create UXM request data
            var requestData = chainTransaction.ToUxmWithdrawalOrderRequestData(merchantNumber);
            var uxmRequest = new SecureRequest<UxmWithdrawalOrderRequest>
            {
                Data = requestData,
                Signature = asymmetricCryptoService.Sign(gameXPrivateKey, requestData)
            };
            
            var result = await uxmService.CreateWithdrawalOrderAsync(uxmRequest);
            
            // Verify UXM signature
            var uxmPublicKey = asymmetricKeyCacheService.UxmPublicKey;
            var isValid =
                asymmetricCryptoService.VerifySignature(uxmPublicKey, result.Data, result.Signature);
            if (!isValid) throw new BadRequestException(MessageCode.System.TokenGenerationFailed, "Invalid signature.");
        }
        catch (Exception ex)
        {
            await chainTransactionRepo.PatchUpdateAsync(chainTransaction.PublicId, x =>
            {
                x.Status = ChainTransactionStatus.Failed;
                x.UpdateMeta(m => m.ErrorMessage = ex.Message);
            }, ct);

            await TryRefundFrozenBalanceAsync(chainTransaction, ct);

            throw;
        }
    }
    
    private async Task TryRefundFrozenBalanceAsync(ChainTransaction chainTransaction, CancellationToken ct)
    {
        UserBalance? balance = chainTransaction.User?.UserBalances.FirstOrDefault(b => b.CryptoTokenId == chainTransaction.CryptoTokenId);
        if (balance == null)
            throw new BadRequestException(MessageCode.Accounting.BalanceNotFound);
        
        const int maxRetries = 3;
        int attempt = 0;
        decimal refundAmount = chainTransaction.Amount + chainTransaction.Fee;

        while (attempt < maxRetries)
        {
            attempt++;
            try
            {
                userBalanceService.Unfreeze(balance, refundAmount);
                await userBalanceRepo.PutUpdateAsync(balance, ct);
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    "[TronWithdrawal] ❌ No. {Attempt} Balance compensation failed，UserId={UserId}, TokenId={TokenId}, OrderNo={OrderNo}, Refund={RefundAmount}, Err={ex}",
                    attempt, 
                    chainTransaction?.UserId ?? string.Empty, 
                    chainTransaction?.CryptoTokenId ?? 0, 
                    chainTransaction?.OrderNumber ?? string.Empty, 
                    refundAmount, 
                    ex);

                await Task.Delay(200, ct); // Retry after a short delay
            }
        }

        logger.LogError(
            "[TronWithdrawal] ❌ Compensation failure exceeds the maximum number of retries，UserId={UserId}, TokenId={TokenId}, OrderNo={OrderNo}, Refund={RefundAmount}",
            chainTransaction?.UserId ?? string.Empty, 
            chainTransaction?.CryptoTokenId ?? 0, 
            chainTransaction?.OrderNumber ?? string.Empty, 
            refundAmount);
        
        throw new BadRequestException(MessageCode.System.TokenGenerationFailed);
    }
}