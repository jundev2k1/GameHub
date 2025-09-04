using game_x.application.Contract.Infrastructure.SignalR.Dtos;

namespace game_x.application.Contract.Infrastructure.SignalR.Services;

public interface ICsAdminHubService
{
    Task SendNotificationAsync(string adminId, NotificationDto message);

    Task SendNotificationToAllAsync(NotificationDto message);
}
