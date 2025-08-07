using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.Services.Wallet;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.Context;

namespace game_x.application.Features.ChainTransactions.Shared.Commands.Callback.CryptoTransactionCallback;

public sealed class CryptoTransactionCallbackHandler(
    IAsymmetricCryptoService asymmetricCryptoService,
    IUnitOfWork unitOfWork,
    IChainTransactionRepo chainTransactionRepo,
    IApplicationEventDispatcher eventDispatcher,
    IUserBalanceService userBalanceService,
    IUserBalanceRepo userBalanceRepo,
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

        UserBalance? balance = transaction.User?.UserBalances.FirstOrDefault(b => b.CryptoTokenId == transaction.CryptoTokenId);
        if (balance == null)
            throw new BadRequestException(MessageCode.Accounting.BalanceNotFound);
        
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
                    await HandleDepositTransactionAsync(requestData, balance, ct);
                    break;
                case ChainTransactionType.Withdrawal:
                    await HandleWithdrawalTransactionAsync(transaction, balance, ct);
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
    
    private async Task HandleDepositTransactionAsync(dynamic requestData, UserBalance balance, CancellationToken ct)
    {
        // Check actualAmount is positive
        if (requestData.ActualAmount <= 0)
            throw new BadRequestException(MessageCode.System.InvalidParameters, "Actual amount must be greater than 0.");
        
        // Add actualAmount to user balance
        userBalanceService.AddAmount(balance, requestData.ActualAmount);
        await userBalanceRepo.PutUpdateAsync(balance, ct);
    }
    
    private async Task HandleWithdrawalTransactionAsync(ChainTransaction transaction, UserBalance balance, CancellationToken ct)
    {
        // Update the user's balance after a successful withdrawal
        userBalanceService.FinalizeFrozen(balance, transaction.Amount);
        await userBalanceRepo.PutUpdateAsync(balance, ct);
    }
}
