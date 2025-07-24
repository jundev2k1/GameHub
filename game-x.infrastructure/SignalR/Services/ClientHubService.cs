using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace game_x.infrastructure.SignalR.Services;

public sealed class ClientHubService(IHubContext<ClientHub, IClientHub> hubContext)
    : IClientHubService
{
    public async Task SendNotificationToMemberAsync(string memberId, NotificationDto message)
    {
        await hubContext.Clients.Group($"member-{memberId}").ReceiveNotification(message);
    }

    public async Task SendToMemberAsync(string memberId, ClientOrderStatusDto orderInfo)
    {
        await hubContext.Clients.Group($"member-{memberId}").OrderUpdated(orderInfo);
    }
}