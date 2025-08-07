using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Features.Notifications.Shared.Commands.MarkAsRead;
using game_x.share.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace game_x.infrastructure.SignalR.Hubs;

public interface IAdminHub
{
    Task ReceiveNotification(NotificationDto message);

    /// <summary>
    ///     Notify that an order has been updated.
    /// </summary>
    /// <param name="transaction">The transaction information that was updated.</param>
    Task TransactionUpdated(AdminTransactionDto transaction);
}

[Authorize(Roles = AppRoles.Admin)]
public sealed class AdminHub(
    ISender sender,
    IUserAccessor userAccessor,
    ILogger<AdminHub> logger) : Hub<IAdminHub>
{
    public const string Path = "/hubs/admin-service";

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (userId.IsNotNullOrEmpty())
            logger.LogInformation($"Admin User connected ({nameof(AdminHub)}): {userId}");

        await Groups.AddToGroupAsync(Context.ConnectionId, $"admin-{userId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation($"Admin User disconnected: {Context.UserIdentifier}");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task MarkNotificationAsRead(Guid notificationId)
    {
        var adminUserId = userAccessor.GetUserId();
        var command = new MarkAsReadCommand(notificationId, adminUserId);
        await sender.Send(command);
    }
}
