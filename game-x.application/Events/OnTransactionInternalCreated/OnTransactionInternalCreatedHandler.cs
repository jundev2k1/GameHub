using System.Text.Json;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUserBalanceUpdated;
using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Events.OnTransactionInternalCreated;

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
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await SendUpdateWalletToMember(targetTransaction.UserId, ct);
            await SendNotificationToAdmin(targetTransaction, ct);
        }, ct);
    }

    private async Task SendNotificationToAdmin(TransactionInternalDto transaction, CancellationToken ct)
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
                transaction.Adapt<AdminTransactionDto>());
        }
    }
    
    private async Task SendUpdateWalletToMember(string userId, CancellationToken ct)
    {
        var @event = new OnUserBalanceUpdatedEvent(userId);
        await eventDispatcher.Publish(@event, ct);
    }
}