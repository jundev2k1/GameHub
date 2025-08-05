using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Features.UserWallet.Dtos;
using game_x.infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace game_x.infrastructure.SignalR.Services;

public sealed class ClientHubService(IHubContext<ClientHub, IClientHub> hubContext)
    : IClientHubService, IHubServices
{
    public async Task SendNotificationToMemberAsync(string memberId, NotificationDto message)
    {
        await hubContext.Clients.Group($"member-{memberId}").ReceiveNotification(message);
    }

    public async Task SendToMemberAsync(string memberId, ClientOrderStatusDto orderInfo)
    {
        await hubContext.Clients.Group($"member-{memberId}").OrderUpdated(orderInfo);
    }
    
    public async Task PushBalanceUpdateAsync(string userId, List<WalletsBaseDto> balance)
    {
        await hubContext.Clients.Group($"member-{userId}").BalanceUpdated(balance);
    }
}