using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Services.Wallet;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.ChainTransactions.Admin.Commands.AdminReviewWithdrawalOrder;

public sealed class AdminReviewWithdrawalOrderHandler(
    IUnitOfWork unitOfWork,
    IUserBalanceService userBalanceService,
    IChainTransactionRepo chainTransactionRepo,
    IApplicationEventDispatcher eventDispatcher,
    IUserBalanceRepo userBalanceRepo,
    IAppLogger<ChainTransaction> logger)
    : ICommandHandler<AdminReviewWithdrawalOrderCommand>
{
    public async Task<Unit> Handle(AdminReviewWithdrawalOrderCommand request, CancellationToken ct = default)
    {
        var transaction = await chainTransactionRepo.GetByIdAsync(request.OrderId, ct) ??
                    throw new NotFoundException(MessageCode.Transaction.TradeNotFound);
        
        // var txAlreadyProcessed = await chainTransactionRepoRepo.GetByIdAsync(request.PublicId, ct);
        // if (txAlreadyProcessed == null)
        // {
        //     logger.LogWarning("Skip duplicate transaction Hash: {TxHash}, UserId: {UserId}, TokenId: {TokenId}", request.TxHash, request.UserId, request.CryptoTokenId);
        //     return Unit.Value;
        // }
        
        if (!transaction.Status.Equals(ChainTransactionStatus.Pending))
            throw new BadRequestException(MessageCode.System.InvalidCurrentStatus);
        
        switch (request.OrderStatus)
        {
            case ChainTransactionStatus.Approved:
                await HandleRejectTransactionAsync(transaction, ct);
                break;
            case ChainTransactionStatus.Rejected:
                await HandleApproveTransactionAsync(transaction, ct);
                break;
            default:
                throw new BadRequestException(MessageCode.System.InvalidParameters);
        }
        
        // await eventDispatcher.Publish(new OnOrderApprovedEvent(transaction!), ct);
        return Unit.Value;
    }

    private async Task HandleRejectTransactionAsync(ChainTransaction transaction, CancellationToken ct)
    {
        await unitOfWork.WithTransactionAsync(
            async () =>
            {
                await chainTransactionRepo
                    .UpdateAsync(transaction.PublicId, updatedOrder => 
                        updatedOrder.UpdateStatus(ChainTransactionStatus.Rejected), ct);

                await TryRefundFrozenBalanceAsync(transaction, ct);
            }, ct);
    }
    
    private async Task HandleApproveTransactionAsync(ChainTransaction transaction, CancellationToken ct)
    {
        await unitOfWork.WithTransactionAsync(
            async () =>
            {
                await chainTransactionRepo
                    .UpdateAsync(transaction.PublicId, updatedOrder => 
                        updatedOrder.UpdateStatus(ChainTransactionStatus.Rejected), ct);

                await TryRefundFrozenBalanceAsync(transaction, ct);
            }, ct);
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