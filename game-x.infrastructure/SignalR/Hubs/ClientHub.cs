using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.LiveStream;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;
using game_x.application.Features.Accounts.User.Dtos;
using game_x.application.Features.Notifications.Shared.Commands.MarkAllAsRead;
using game_x.application.Features.Notifications.Shared.Commands.MarkAsRead;
using game_x.share.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using game_x.application.Common.Abstractions.Events;
using game_x.application.Events.Rewards.OnDailyCheckIn;
using game_x.application.Features.Rewards.Dtos;
using game_x.infrastructure.SignalR.Groups;

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

    Task OnReceiveLiveStreamingShortcuts(LiveStreamShortcutInfo[] streamInfo);

    Task NotifyWalletSynchronizationFailed(Guid platformId);
    /// <summary>Notify when member transfers or received money between friends.</summary>
    Task TransactionTransfer(TransactionTransferSignalDto orderInfo);
    Task RevokeRefreshToken(string userId);
    Task InventoryUpdated(UserInventoryDto[] dto);
}

[Authorize(Roles = $"{AppRoles.Talent},{AppRoles.User}")]
public sealed class ClientHub(
    ISender sender,
    IFileManagerCacheService fileManagerCache,
    ILiveStreamManagerCacheService liveStreamManager,
    IApplicationEventDispatcher dispatcher,
    IAppLogger<ClientHub> logger) : Hub<IClientHub>
{
    public const string Path = "/hubs/client-service";

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (userId.IsNotNullOrEmpty())
        {
            logger.LogInformation($"User connected ({nameof(ClientHub)}): {userId}");
            await dispatcher.Publish(new OnDailyCheckInEvent(userId!));
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, ActorGroups.Member(userId!));
        await Groups.AddToGroupAsync(Context.ConnectionId, ActorGroups.Broadcast(AppRoles.User));
        await base.OnConnectedAsync();

        // Send live-streaming shortcuts to live-streaming talent
        if (userId.IsNotNullOrEmpty())
            await HandleSendLiveStreamShortcut(userId!);
    }

    private async Task HandleSendLiveStreamShortcut(string userId)
    {
        if (Context.User?.Identity?.IsAuthenticated ?? false) return;

        var roles = Context.User?
            .FindAll(ClaimTypes.Role)
            .Select(r => r.Value)
            .ToList() ?? [];
        if (!roles.Contains(AppRoles.Talent)) return;

        var streamList = liveStreamManager.GetAllStreamKeys();
        if (!streamList.TryGetValue(userId, out var activeStreamKeys))
            return;

        var streamStatusTaskList = activeStreamKeys
            .Select(async streamKey =>
            {
                var streamInfo = liveStreamManager.GetLiveStreamStatus(streamKey);
                if (streamInfo == null) return null;

                streamInfo.Thumbnail = await fileManagerCache.GetFileUrl(streamInfo.ThumbnailId);
                return streamInfo.Adapt<LiveStreamShortcutInfo>();
            })
            .ToArray();
        var streamStatusList = await Task.WhenAll(streamStatusTaskList);
        streamStatusList = [.. streamStatusList.Where(i => i is not null)];
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