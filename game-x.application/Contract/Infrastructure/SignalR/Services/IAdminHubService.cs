using game_x.application.Contract.Infrastructure.SignalR.Dtos;

namespace game_x.application.Contract.Infrastructure.SignalR.Services;

public interface IAdminHubService
{
    Task SendNotificationToAdminAsync(string adminId, NotificationDto message);

    Task SendNotificationToAllAsync(NotificationDto message);

    Task SendOrderStatusToAdminAsync(string adminId, AdminOrderStatusDto orderInfo);
}
