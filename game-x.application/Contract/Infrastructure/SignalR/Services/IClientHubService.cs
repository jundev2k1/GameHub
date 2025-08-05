using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Features.UserWallet.Dtos;

namespace game_x.application.Contract.Infrastructure.SignalR.Services;

public interface IClientHubService
{
    Task SendNotificationToMemberAsync(string memberId, NotificationDto message);
    Task SendToMemberAsync(string memberId, ClientOrderStatusDto orderInfo);
    Task PushBalanceUpdateAsync(string userId, List<WalletsBaseDto> balances);
}
