using game_x.application.Contract.Infrastructure.SignalR.Dtos;

namespace game_x.application.Contract.Infrastructure.SignalR.Services;

public interface IAdminHubService
{
    Task SendNotificationAsync(string adminId, NotificationDto message);

    Task SendNotificationToAllAsync(NotificationDto message);

    Task SendTransactionToAdminAsync(string adminId, AdminTransactionDto transaction);
}
