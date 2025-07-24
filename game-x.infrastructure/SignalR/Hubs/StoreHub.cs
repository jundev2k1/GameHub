using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.share.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace game_x.infrastructure.SignalR.Hubs;

public interface IStoreHub
{
    /// <summary>
    /// Notify that an order has been updated.
    /// </summary>
    /// <param name="orderInfo">The order information that was updated.</param>
    Task OrderUpdated(StaffOrderStatusDto orderInfo);
}

[Authorize(Roles = AppRoles.Staff)]
public sealed class StoreHub(ILogger<StoreHub> logger, IHeartBeatCacheService heartBeatCache) : Hub<IStoreHub>
{
    public const string Path = "/hubs/store-service";

    public override async Task OnConnectedAsync()
    {
        var staffId = Context.UserIdentifier;
        if (staffId.IsNotNullOrEmpty())
        {
            logger.LogInformation("Staff User connected({hubName}): {staffId}", nameof(StoreHub), staffId);
            SetOnline();
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"staff-{staffId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var staffId = Context.UserIdentifier;
        if (staffId.IsNotNullOrEmpty())
        {
            logger.LogInformation("Staff User disconnected: {staffId}", staffId);
            SetOffline();
        }

        await base.OnDisconnectedAsync(exception);
    }

    private void SetOnline() => UpdateHeartBeatState(true);

    private void SetOffline() => UpdateHeartBeatState(false);

    private void UpdateHeartBeatState(bool isOnline)
    {
        var staffId = Context.UserIdentifier.ToStringOrEmpty();
        var sessionId = Context.GetHttpContext()?.Request.Query["sessionId"].ToStringOrEmpty();
        var targetHeartBeat = heartBeatCache.GetHeartBeatList()
            .FirstOrDefault(item => item.Id.StartsWith("heartbeat:") && item.Id.EndsWith($":staff:{staffId}:session:{sessionId}"));
        if (targetHeartBeat is null) return;

        heartBeatCache.UpdateStatus(targetHeartBeat.Id, isOnline);
    }
}
