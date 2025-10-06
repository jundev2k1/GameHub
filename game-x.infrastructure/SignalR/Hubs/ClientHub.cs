using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.User.Dtos;
using game_x.application.Features.LiveStreams.Streaming.Dtos;
using game_x.application.Features.Notifications.Shared.Commands.MarkAllAsRead;
using game_x.application.Features.Notifications.Shared.Commands.MarkAsRead;
using game_x.share.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace game_x.infrastructure.SignalR.Hubs;

public interface IClientHub
{
    Task ReceiveNotification(NotificationDto message);

    /// <summary>Notify that an order has been updated.</summary>
    /// <param name="orderInfo">The order information that was updated.</param>
    Task TransactionUpdated(ClientTransactionDto orderInfo);

    /// <summary>Notify that user wallets have been updated.</summary>
    Task WalletsUpdated(ClientWalletsDto dto);

    Task UserVerifyUpdated(VerificationStatusDto dto);

    Task GameBalanceUpdated(GameBalanceNotificationDto notificationDto);

    Task OnReceiveLiveStreamingShortcuts(LiveStreamStatusDto[] streamInfo);
}

[Authorize(Roles = AppRoles.User)]
public sealed class ClientHub(
    ISender sender,
    ILiveStreamRepo liveStreamRepo,
    ILiveStreamManagerCacheService liveStreamManager,
    IAppLogger<ClientHub> logger) : Hub<IClientHub>
{
    public const string Path = "/hubs/client-service";

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (userId.IsNotNullOrEmpty())
            logger.LogInformation($"User connected ({nameof(ClientHub)}): {userId}");

        await Groups.AddToGroupAsync(Context.ConnectionId, $"member-{userId}");

        await base.OnConnectedAsync();

        // Send live streaming shortcuts to live streaming talent
        if (userId.IsNotNullOrEmpty())
            await HandleSendLiveStreamShortcut(userId!);
    }

    private async Task HandleSendLiveStreamShortcut(string userId)
    {
        var streamList = await liveStreamRepo.GetsByTalentIdAsync(userId);
        var streamStatusList = streamList
            .Select(s => liveStreamManager.GetLiveStreamStatus(s.StreamKey))
            .Where(s => s is not null)
            .ToArray();
        if (streamStatusList.Length == 0) return;

        await Clients.Caller.OnReceiveLiveStreamingShortcuts(streamStatusList!);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation($"User disconnected: {Context.UserIdentifier}");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task MarkNotificationAsRead(Guid notificationId)
    {
        var adminUserId = Context.UserIdentifier!;
        var command = new MarkAsReadCommand(notificationId, adminUserId);
        await sender.Send(command);
    }

    public async Task MarkAllNotificationsAsRead()
    {
        var adminUserId = Context.UserIdentifier!;
        var command = new MarkAllAsReadCommand(adminUserId);
        await sender.Send(command);
    }
}