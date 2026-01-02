using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;
using game_x.application.Features.BankAccountVerifications.Dtos;
using game_x.application.Features.Kyc.Dtos;
using game_x.application.Features.Notifications.Shared.Commands.MarkAllAsRead;
using game_x.application.Features.Notifications.Shared.Commands.MarkAsRead;
using game_x.infrastructure.SignalR.Groups;
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
    Task KycCreated(UserKycListItemDto verify);
    Task BankAccountCreated(BankAccountListItemDto verify);
    Task TransactionReviewed(AdminOrderReviewedDto order);
    Task KycReviewed(AdminOrderReviewedDto order);
    Task BankAccountReviewed(AdminOrderReviewedDto order);
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

        await Groups.AddToGroupAsync(Context.ConnectionId, ActorGroups.Admin(userId!));
        await Groups.AddToGroupAsync(Context.ConnectionId, ActorGroups.Broadcast(AppRoles.Admin));
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
    
    public async Task MarkAllNotificationsAsRead()
    {
        var adminUserId = Context.UserIdentifier!;
        var command = new MarkAllAsReadCommand(adminUserId);
        await sender.Send(command);
    }
}