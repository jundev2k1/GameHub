using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Features.Notifications.Shared.Commands.MarkAllAsRead;
using game_x.application.Features.Notifications.Shared.Commands.MarkAsRead;
using game_x.share.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace game_x.infrastructure.SignalR.Hubs;

public interface IClientHub
{
    Task ReceiveNotification(NotificationDto message);

    /// <summary>Notify that an order has been updated.</summary>
    /// <param name="orderInfo">The order information that was updated.</param>
    Task TransactionUpdated(ClientTransactionDto orderInfo);
    /// <summary>Notify that user balance has been updated.</summary>
    Task BalanceUpdated(ClientBalanceDto dto);
    /// <summary>Notify that a transaction history has been updated.</summary>
    Task LedgerUpdated(ClientLedgerDto dto);
    Task UserKycUpdated(UserKycDto dto);
    Task UserBankAccountUpdated(UserBankAccountDto dto);
}

[Authorize(Roles = AppRoles.User)]
public sealed class ClientHub(
    ISender sender,
    ILogger<ClientHub> logger,
    IUserAccessor userAccessor) : Hub<IClientHub>
{
    public const string Path = "/hubs/client-service";

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (userId.IsNotNullOrEmpty())
            logger.LogInformation($"User connected ({nameof(ClientHub)}): {userId}");

        await Groups.AddToGroupAsync(Context.ConnectionId, $"member-{userId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation($"User disconnected: {Context.UserIdentifier}");
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
        var adminUserId = userAccessor.GetUserId();
        var command = new MarkAllAsReadCommand(adminUserId);
        await sender.Send(command);
    }
}