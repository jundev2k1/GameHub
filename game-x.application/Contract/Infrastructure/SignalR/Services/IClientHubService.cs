using game_x.application.Contract.Infrastructure.SignalR.Dtos;

namespace game_x.application.Contract.Infrastructure.SignalR.Services;

public interface IClientHubService
{
    Task SendNotificationToMemberAsync(string memberId, NotificationDto message);

    Task SendToMemberAsync(string memberId, ClientOrderStatusDto orderInfo);
}
