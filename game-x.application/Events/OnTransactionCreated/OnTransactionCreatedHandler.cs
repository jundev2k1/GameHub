using System.Text.Json;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUserBalanceUpdated;

namespace game_x.application.Events.OnTransactionCreated;

public sealed class OnTransactionCreatedHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    INotificationRepo notificationRepo,
    IAdminHubService adminHubService,
    IApplicationEventDispatcher eventDispatcher) : IApplicationEventHandler<OnTransactionCreatedEvent>
{
    public async Task Handle(OnTransactionCreatedEvent @event, CancellationToken ct = default)
    {
        var targetTransaction = @event.Transaction;
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await SendUpdateWalletToMember(targetTransaction, ct);
            await SendNotificationToAdmin(targetTransaction, ct);
        }, ct);
    }

    private async Task SendNotificationToAdmin(ChainTransaction transaction, CancellationToken ct)
    {
        var adminUsers = await userRepo.GetAdminUsers(ct);

        var metadata = JsonSerializer.Serialize(transaction.Adapt<TransactionNotificationDto>());
        foreach (var adminUser in adminUsers)
        {
            var notification = Notification.Create(
                NotificationMessageKey.Transaction_Created,
                adminUser.Id,
                NotificationType.Transaction,
                NotificationSeverity.Success,
                metadata);
            await notificationRepo.AddNotificationAsync(notification, ct);

            // Send notification to all the admin
            await adminHubService.SendNotificationAsync(
                adminUser.Id,
                notification.Adapt<NotificationDto>());

            // Send transaction to all the admin
            await adminHubService.SendTransactionToAdminAsync(
                adminUser.Id,
                new AdminTransactionDto(
                    TransactionId: transaction.PublicId,
                    Status: transaction.Status.ToString(),
                    Type: transaction.Type.ToString()));
        }
    }
    
    private async Task SendUpdateWalletToMember(ChainTransaction transaction, CancellationToken ct)
    {
        if (transaction.UserId != null)
        {
            var @event = new OnUserBalanceUpdatedEvent(transaction.UserId);
            await eventDispatcher.Publish(@event, ct);
        }
    }
}