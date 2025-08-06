using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.Context;

namespace game_x.application.Features.ChainTransactions.Shared.Commands.Callback.CryptoTransactionCallback;

public sealed class CryptoTransactionCallbackHandler(
    IAsymmetricCryptoService asymmetricCryptoService,
    IUnitOfWork unitOfWork,
    IChainTransactionRepo chainTransactionRepo,
    IApplicationEventDispatcher eventDispatcher,
     IAsymmetricKeyCacheService asymmetricKeyCacheService)
    : ICommandHandler<CryptoTransactionCallbackCommand, CryptoTransactionCallbackResult>
{
    public async Task<CryptoTransactionCallbackResult> Handle(CryptoTransactionCallbackCommand request,
        CancellationToken ct = default)
    {
        var (requestData, signature) = request;
        
        // Verify UXM signature
        var uxmPublicKey = asymmetricKeyCacheService.UxmPublicKey;
        var isValid = asymmetricCryptoService.VerifySignature(uxmPublicKey, requestData, signature);
        if (!isValid)
            throw new BadRequestException(MessageCode.System.TokenGenerationFailed, "Invalid signature.");;

        ChainTransaction? transaction = 
            await chainTransactionRepo.GetByOrderNumberAsync(requestData.OrderNumber ?? string.Empty, ct);
        
        if(transaction == null)
            throw new NotFoundException(MessageCode.Transaction.TradeNotFound,
                $"Transaction with order number '{requestData.OrderNumber}' not found.");
            
        // Anti-spam request if the Transaction has already been updated
        if (transaction.Status == ChainTransactionStatus.Completed)
            throw new BadRequestException(MessageCode.System.InvalidCurrentStatus);

        await unitOfWork.WithTransactionAsync(async () =>
        {
            await chainTransactionRepo.PatchUpdateAsync(transaction.PublicId, order =>
            {
                order.UpdateStatus(ChainTransactionStatus.Completed);
                order.UpdateUxmResponse(
                    actualAmount: requestData.ActualAmount,
                    orderUid: requestData.OrderUid ?? string.Empty,
                    hash: requestData?.Hash ?? string.Empty,
                    confirmedAt: requestData?.ConfirmedAt
                );
            }, ct);
            
            switch (transaction.Type)
            {
                case ChainTransactionType.Deposit:
                    await HandleDepositTransactionAsync(transaction, ct);
                    break;
                case ChainTransactionType.Withdrawal:
                    await HandleWithdrawalTransactionAsync(transaction, ct);
                    break;
                default:
                    throw new BadRequestException(MessageCode.System.InvalidParameters);
            }
            
            // Set order for event publishing (Send real-time message to staff and all the admin)
        }, ct);
        
        // Ensure the audit log records the order status updated by the external API
        using (AuditSourceContext.Use(AuditSource.External))
        {
            await unitOfWork.SaveChangesAsync(ct);
        }
 
        return new CryptoTransactionCallbackResult(
            $"Order ({requestData.OrderUid}) updated successfully.");
    }
    
    private async Task HandleDepositTransactionAsync(ChainTransaction transaction, CancellationToken ct)
    {
        await unitOfWork.WithTransactionAsync(
            async () =>
            {
                
            }, ct);
    }
    
    private async Task HandleWithdrawalTransactionAsync(ChainTransaction transaction, CancellationToken ct)
    {
        
    }
}
