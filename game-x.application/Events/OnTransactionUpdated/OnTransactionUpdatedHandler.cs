using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUserBalanceUpdated;
using game_x.application.Features.Transactions.Dtos;

namespace game_x.application.Events.OnTransactionUpdated;

public sealed class OnTransactionUpdatedHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    ITransactionRepo transactionRepo,
    INotificationRepo notificationRepo,
    IClientHubService clientHubService,
    IAdminHubService adminHubService,
    IApplicationEventDispatcher eventDispatcher) : IApplicationEventHandler<OnTransactionUpdatedEvent>
{
    public async Task Handle(OnTransactionUpdatedEvent @event, CancellationToken ct = default)
    {
        var transaction = await transactionRepo.GetInternalByIdAsync(@event.TransactionId, ct);
        var txDto = transaction.Adapt<TransactionInternalDto>();

        // Create and send notification, transaction for admin
        await SendNotificationToAdmin(txDto, ct);

        // Refresh balance for target user
        var balanceUpdatedEvent = new OnUserBalanceUpdatedEvent(transaction.UserId);
        await eventDispatcher.Publish(balanceUpdatedEvent, ct);
    }

    private async Task SendNotificationToAdmin(TransactionInternalDto transaction, CancellationToken ct)
    {
        var adminUsers = await userRepo.GetAdminUsers(ct);
        var notifications = adminUsers.Select(u => Notification.Create(
            NotificationMessageKey.Transaction_Cancelled,
            u.Id,
            NotificationType.Transaction,
            NotificationSeverity.Error));
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await notificationRepo.AddRangeNotificationsAsync(notifications, ct);
        }, ct);

        // Send notification to all the admin
        var adminNotification = notifications.FirstOrDefault();
        if (adminNotification != null)
        {
            await adminHubService.SendNotificationAsync(
                adminNotification.UserId!,
                adminNotification.Adapt<NotificationDto>());
        }

        // Send transaction to all the admin
        var adminTxDto = transaction.Adapt<AdminTransactionDto>();
        await adminHubService.SendTransactionToAllAdminAsync(adminTxDto);

        // Send transaction to target user
        var clientTxDto = transaction.Adapt<ClientTransactionDto>();
        await clientHubService.SendTransactionToMemberAsync(transaction.UserId, clientTxDto);
    }
}