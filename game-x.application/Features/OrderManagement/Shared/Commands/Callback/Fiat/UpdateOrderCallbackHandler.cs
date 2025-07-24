using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnOrderCompleted;
using game_x.share.Context;

namespace game_x.application.Features.OrderManagement.Shared.Commands.Callback.Fiat;

public sealed class UpdateOrderCallbackHandler(
    IAsymmetricKeyRepo asymmetricKeyRepo,
    IAsymmetricCryptoService asymmetricCryptoService,
    IUnitOfWork unitOfWork,
    IOrderRepo orderRepo,
    IApplicationEventDispatcher eventDispatcher)
    : ICommandHandler<UpdateOrderCallbackCommand, UpdateOrderCallbackResult>
{
    public async Task<UpdateOrderCallbackResult> Handle(UpdateOrderCallbackCommand request,
        CancellationToken ct = default)
    {
        // 1. Verify UXM signature
        var (requestData, signature) = request;
        var uxmPublicKey = await asymmetricKeyRepo
            .GetByCompositeKeyAsync(AsymmetricKeyNames.Uxm, KeyType.Public, AsymmetricType.ECDSA, ct);
        if (uxmPublicKey is null) throw new NotFoundException("UXM Public Key is not found.");

        var isValid = asymmetricCryptoService.VerifySignature(uxmPublicKey.KeyValue, requestData, signature);
        if (!isValid) throw new BadRequestException("Invalid signature.");

        // 2. Update Order Status returns from callback
        Order? updateOrder = null;
        await orderRepo.UpdateAsync(requestData.OrderUid, order =>
        {
            // Anti-spam request if the Order has already been updated
            if (order.OrderStatus.Value == OrderStatus.Completed.Value)
                throw new Exception("Unable to update completed order.");

            // Update status to wait for reviewing by admin
            order.UpdateStatus(OrderStatus.Completed);

            // Set order for event publishing (Send real-time message to staff and all the admin)
            updateOrder = order;
        }, ct);

        // Ensure the audit log records the order status updated by the external API
        using (AuditSourceContext.Use(AuditSource.External))
        {
            await unitOfWork.SaveChangesAsync(ct);
        }

        if (updateOrder is null)
            throw new NotFoundException($"Order ({requestData.OrderUid}) update failed.");

        // 3. Publish Order Completed event
        await eventDispatcher.Publish(new OnOrderCompletedEvent(updateOrder!), ct);

        return new UpdateOrderCallbackResult(
            $"Order ({requestData.OrderUid}) updated successfully.");
    }
}