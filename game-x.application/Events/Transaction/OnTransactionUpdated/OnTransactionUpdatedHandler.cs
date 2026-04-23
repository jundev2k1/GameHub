using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.Account.OnUserBalanceUpdated;
using game_x.application.Features.Transactions.Dtos;

namespace game_x.application.Events.Transaction.OnTransactionUpdated;

public sealed class OnTransactionUpdatedHandler(
    IUnitOfWork unitOfWork,
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

        // Create notification in DB
        var notification = await CreateNotificationAsync(txDto, ct);

        // Send notification to all the admin
        await adminHubService.SendNotificationAsync(
            transaction.UserId,
            notification.Adapt<NotificationDto>());

        // Send transaction to all the admin
        var adminTxDto = txDto.Adapt<AdminTransactionDto>();
        await adminHubService.SendTransactionToAllAdminAsync(adminTxDto);

        // Send transaction to target user
        var clientTxDto = txDto.Adapt<ClientTransactionDto>();
        await clientHubService.SendTransactionToMemberAsync(transaction.UserId, clientTxDto);

        // Refresh balance for target user
        var balanceUpdatedEvent = new OnUserBalanceUpdatedEvent(transaction.UserId);
        await eventDispatcher.Publish(balanceUpdatedEvent, ct);
    }

    private async Task<Notification> CreateNotificationAsync(TransactionInternalDto transaction, CancellationToken ct)
    {
        // Create notification for the user whom is own target transaction
        var notification = Notification.Create(
            NotificationMessageKey.Transaction_Cancelled,
            transaction.UserId,
            NotificationType.Transaction,
            NotificationSeverity.Error);
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await notificationRepo.AddNotificationAsync(notification, ct);
        }, ct);

        return notification;
    }
}