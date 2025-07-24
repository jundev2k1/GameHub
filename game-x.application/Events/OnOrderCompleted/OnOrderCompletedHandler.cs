using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.OrderManagement.Dtos;
using System.Text.Json;

namespace game_x.application.Events.OnOrderCompleted;

public sealed class OnOrderCompletedHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    INotificationRepo notificationRepo,
    IStoreHubService storeHubService,
    IAdminHubService adminHubService,
    IClientHubService clientHubService) : IApplicationEventHandler<OnOrderCompletedEvent>
{
    public async Task Handle(OnOrderCompletedEvent @event, CancellationToken ct = default)
    {
        var targetOrder = @event.Order;
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await SendToStaff(targetOrder, ct);
            await SendToAdmin(targetOrder, ct);
            await SendToMember(targetOrder, ct);
        }, ct);
    }

    private async Task SendToStaff(Order order, CancellationToken ct)
    {
        // Create Notification
        var notification = Notification.Create(
            NotificationMessageKey.Order_Completed,
            order.StaffId,
            NotificationType.Order,
            NotificationSeverity.Success,
            JsonSerializer.Serialize(order.Adapt<OrderDto>()));
        await notificationRepo.AddNotificationAsync(notification, ct);

        // Send to staff counter-standing
        await storeHubService.SendOrderStatusToStaffAsync(
            order.StaffId,
            new StaffOrderStatusDto(order.UserId, order.PublicId, order.OrderStatus.Value));
    }

    private async Task SendToAdmin(Order order, CancellationToken ct)
    {
        var adminUsers = await userRepo.GetAdminUsers(ct);

        foreach (var adminUser in adminUsers)
        {
            // Create notification
            var notification = Notification.Create(
                NotificationMessageKey.Order_Completed,
                adminUser.Id,
                NotificationType.Order,
                NotificationSeverity.Success,
                JsonSerializer.Serialize(order.Adapt<OrderDto>()));
            await notificationRepo.AddNotificationAsync(notification, ct);

            // Send notification to all the admin
            await adminHubService.SendNotificationToAdminAsync(
                adminUser.Id,
                notification.Adapt<NotificationDto>());

            // Send order status to all the admin
            await adminHubService.SendOrderStatusToAdminAsync(
                adminUser.Id,
                new AdminOrderStatusDto(order.PublicId, order.OrderStatus.Value));
        }
    }

    private async Task SendToMember(Order order, CancellationToken ct)
    {
        // Create notification
        var notification = Notification.Create(
            NotificationMessageKey.Order_Completed,
            order.UserId,
            NotificationType.Order,
            NotificationSeverity.Success,
            JsonSerializer.Serialize(order.Adapt<OrderDto>()));
        await notificationRepo.AddNotificationAsync(notification, ct);

        // Send notification to the member
        await clientHubService.SendNotificationToMemberAsync(
            order.UserId,
            notification.Adapt<NotificationDto>());

        // Sent to the member
        await clientHubService.SendToMemberAsync(
            order.UserId,
            new ClientOrderStatusDto(order.PublicId, order.OrderStatus.Value));
    }
}
