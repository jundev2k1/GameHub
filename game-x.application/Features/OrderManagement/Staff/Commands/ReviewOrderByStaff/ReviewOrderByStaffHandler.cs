using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnOrderApproved;

namespace game_x.application.Features.OrderManagement.Staff.Commands.ReviewOrderByStaff;

public sealed class ReviewOrderByStaffHandler(
    IUnitOfWork unitOfWork,
    IOrderRepo orderRepo,
    IApplicationEventDispatcher eventDispatcher)
    : ICommandHandler<ReviewOrderByStaffCommand>
{
    public async Task<Unit> Handle(ReviewOrderByStaffCommand request, CancellationToken ct = default)
    {
        var order = await orderRepo.GetByOrderIdAsync(request.OrderId, ct) ??
            throw new NotFoundException("Order not found");
        
        if (!order.OrderStatus.Equals(OrderStatus.Completed))
            throw new BadRequestException("Current status is invalid.");
        
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await orderRepo.UpdateAsync(
                request.OrderId,
                updatedOrder => updatedOrder.UpdateStatus(OrderStatus.Of(request.OrderStatus)), ct);
        }, ct);

        order.UpdateStatus(OrderStatus.Of(request.OrderStatus));
        await eventDispatcher.Publish(new OnOrderApprovedEvent(order!), ct);

        return Unit.Value;
    }
}
