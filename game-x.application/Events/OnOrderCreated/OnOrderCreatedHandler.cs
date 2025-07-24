using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.OrderManagement.Dtos;
using System.Text.Json;

namespace game_x.application.Events.OnOrderCreated;

public sealed class OnOrderCreatedHandler(
    IUnitOfWork unitOfWork,
    INotificationRepo notificationRepo,
    IClientHubService clientHubService) : IApplicationEventHandler<OnOrderCreatedEvent>
{
    public async Task Handle(OnOrderCreatedEvent @event, CancellationToken ct = default)
    {
        var targetOrder = @event.Order;
        await unitOfWork.WithTransactionAsync(async () => { await SendToMember(targetOrder, ct); }, ct);
    }

    private async Task SendToMember(Order order, CancellationToken ct)
    {
        // Create notification
        var notification = Notification.Create(
            NotificationMessageKey.Order_Created,
            order.UserId,
            NotificationType.Order,
            NotificationSeverity.Success,
            JsonSerializer.Serialize(order.Adapt<OrderDto>()));
        await notificationRepo.AddNotificationAsync(notification, ct);

        // Send notification to all the admin
        await clientHubService.SendNotificationToMemberAsync(
            order.UserId,
            notification.Adapt<NotificationDto>());

        // Sent to the member
        await clientHubService.SendToMemberAsync(
            order.UserId,
            new ClientOrderStatusDto(order.PublicId, order.OrderStatus.Value));
    }
}
