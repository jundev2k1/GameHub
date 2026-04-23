using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.Account.OnUserBalanceUpdated;
using game_x.application.Features.Transactions.Dtos;
using System.Text.Json;

namespace game_x.application.Events.Transaction.OnTransactionInternalCreated;

public sealed class OnTransactionInternalCreatedHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    INotificationRepo notificationRepo,
    IAdminHubService adminHubService,
    IApplicationEventDispatcher eventDispatcher) : IApplicationEventHandler<OnTransactionInternalCreatedEvent>
{
    public async Task Handle(OnTransactionInternalCreatedEvent @event, CancellationToken ct = default)
    {
        var targetTransaction = @event.Transaction;

        IEnumerable<Notification> notifications = [];
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await SendUpdateWalletToMember(targetTransaction.UserId, ct);
            notifications = await CreateAdminNotifications(targetTransaction, ct);
            await notificationRepo.AddRangeNotificationsAsync(notifications, ct);
        }, ct);

        // Send notification to all the admin
        var adminNotification = notifications.FirstOrDefault();
        if (adminNotification != null)
        {
            await adminHubService.SendNotificationToAllAsync(
                adminNotification.Adapt<NotificationDto>());
        }

        // Send transaction to all the admin
        await adminHubService.SendTransactionToAllAdminAsync(
            targetTransaction.Adapt<AdminTransactionDto>());
    }

    private async Task<IEnumerable<Notification>> CreateAdminNotifications(TransactionInternalDto transaction, CancellationToken ct)
    {
        var adminUsers = await userRepo.GetAdminUsers(ct);

        var metadata = JsonSerializer.Serialize(transaction.Adapt<TransactionNotificationDto>());
        var notifications = adminUsers.Select(u => Notification.Create(
            NotificationMessageKey.Transaction_Created,
            u.Id,
            NotificationType.Transaction,
            NotificationSeverity.Success,
            metadata));
        return notifications;
    }

    private async Task SendUpdateWalletToMember(string userId, CancellationToken ct)
    {
        var @event = new OnUserBalanceUpdatedEvent(userId);
        await eventDispatcher.Publish(@event, ct);
    }
}
