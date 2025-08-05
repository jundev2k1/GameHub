using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace game_x.infrastructure.SignalR.Services;

public sealed class AdminHubService(IHubContext<AdminHub, IAdminHub> hubContext)
    : IAdminHubService, IHubServices
{
    public async Task SendNotificationToAdminAsync(string adminId, NotificationDto message)
    {
        await hubContext.Clients.Group($"admin-{adminId}").ReceiveNotification(message);
    }

    public async Task SendNotificationToAllAsync(NotificationDto message)
    {
        await hubContext.Clients.All.ReceiveNotification(message);
    }

    public async Task SendOrderStatusToAdminAsync(string adminId, AdminOrderStatusDto orderInfo)
    {
        await hubContext.Clients.Group($"admin-{adminId}").OrderUpdated(orderInfo);
    }
}
