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
    IOptions<GameXSettings> gameXSettings,
    IAsymmetricKeyCacheService asymmetricKeyCacheService,
    IAsymmetricCryptoService asymmetricCryptoService)
    : ICommandHandler<AdminReviewWithdrawalOrderCommand>
{
    public async Task<Unit> Handle(AdminReviewWithdrawalOrderCommand request, CancellationToken ct = default)
    {
        var transaction = await chainTransactionRepo.GetByIdAsync(request.OrderId ?? Guid.Empty, ct);

        // The transaction already exists on the blockchain
        if (transaction.Hash?.IsNotNullOrEmpty() == true)
        {
            logger.LogWarning("Skip duplicate transaction Hash: {TxHash}, UserId: {UserId}, TokenId: {TokenId}", 
                transaction.Hash, transaction.UserId ?? String.Empty, transaction.CryptoTokenId.ToString());
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
        transaction.UpdateStatus(ChainTransactionStatus.Approved);
        await chainTransactionRepo.PutUpdateAsync(transaction, ct);
        
        await SendUxmWithdrawalOrderAsync(transaction, ct);
    }
    
    private async Task HandleRejectTransactionAsync(ChainTransaction transaction, CancellationToken ct)
    {
        transaction.UpdateStatus(ChainTransactionStatus.Rejected);
        
        await unitOfWork.WithTransactionAsync(
            async () =>
            {
                await chainTransactionRepo.PutUpdateAsync(transaction, ct);
                await TryRefundFrozenBalanceAsync(transaction, ct);
            }, ct);
    }
    
    private async Task SendUxmWithdrawalOrderAsync(ChainTransaction chainTransaction, CancellationToken ct)
    {     
        try
        {
            var gameXPrivateKey = asymmetricKeyCacheService.GameXPrivateKey;
            var merchantNumber = gameXSettings.Value.MerchantNumber;
        
            // Create UXM request data
            var requestData = chainTransaction.ToUxmWithdrawalOrderRequest(merchantNumber);
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

        decimal refundAmount = chainTransaction.TotalAmount;

        try
        {
            userBalanceService.Unfreeze(balance, refundAmount);
            await userBalanceRepo.PutUpdateAsync(balance, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(
                "[TronWithdrawal] ❌ Balance compensation failed，UserId={UserId}, TokenId={TokenId}, OrderNo={OrderNo}, Refund={RefundAmount}, Err={ex}",
                chainTransaction.UserId ?? string.Empty, 
                chainTransaction.CryptoTokenId, 
                chainTransaction.OrderNumber, 
                refundAmount, 
                ex);

            throw new BadRequestException(MessageCode.Accounting.InsufficientFrozenBalance);
        }
    }
}