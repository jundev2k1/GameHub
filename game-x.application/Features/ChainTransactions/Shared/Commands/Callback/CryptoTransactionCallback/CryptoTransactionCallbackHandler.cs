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
        
        // 1. Verify UXM signature
        var uxmPublicKey = asymmetricKeyCacheService.UxmPublicKey;
        var isValid =
            asymmetricCryptoService.VerifySignature(uxmPublicKey, requestData, signature);
        if (!isValid) throw new BadRequestException("Invalid signature.");

        // 2. Update Order Status returns from callback
        ChainTransaction? updateChainTransaction = null;

        Guid orderUid = Guid.Parse(requestData.OrderUid);

        await chainTransactionRepo.UpdateAsync(orderUid, order =>
        {
            // Anti-spam request if the Order has already been updated
            //   if (order.Status.Value == OrderStatus.Completed.Value)
            //      throw new Exception("Unable to update completed order.");

            // Update status to wait for reviewing by admin
            order.UpdateStatus(ChainTransactionStatus.Completed);

            // Set order for event publishing (Send real-time message to staff and all the admin)
            updateChainTransaction = order;
        }, ct);

        // Ensure the audit log records the order status updated by the external API
        using (AuditSourceContext.Use(AuditSource.External))
        {
            await unitOfWork.SaveChangesAsync(ct);
        }

        if (updateChainTransaction is null)
            throw new NotFoundException($"Order ({requestData.OrderUid}) update failed.");

        // 3. Publish Order Completed event
        // await eventDispatcher.Publish(new OnChainTransactionCompletedEvent(updateOrder!), ct);

        return new CryptoTransactionCallbackResult(
            $"Order ({requestData.OrderUid}) updated successfully.");
    }
}
