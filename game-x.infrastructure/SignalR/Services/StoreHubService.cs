using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace game_x.infrastructure.SignalR.Services;

public sealed class StoreHubService(IHubContext<StoreHub, IStoreHub> hubContext)
    : IStoreHubService
{
    public async Task SendOrderStatusToStaffAsync(string staffId, StaffOrderStatusDto orderInfo)
    {
        await hubContext.Clients.Group($"staff-{staffId}").OrderUpdated(orderInfo);
    }
}
