using System.Text.Json;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.OnTransactionTransferred;

public sealed class OnTransactionTransferredHandler(
    IUnitOfWork unitOfWork,
    INotificationRepo notificationRepo,
    IClientHubService clientHubService) : IApplicationEventHandler<OnTransactionTransferredEvent>
{
    public async Task Handle(OnTransactionTransferredEvent @event, CancellationToken ct = default)
    {
        await SendToMember(@event.Dto, ct);
    }

    private async Task SendToMember(TransactionTransferSignalDto tx, CancellationToken ct)
    {
        if (tx.Type == TransactionType.TransferReceived && tx.ReceiverId != null)
        {
            var notification = Notification.Create(
                NotificationMessageKey.Transaction_Received,
                tx.ReceiverId,
                NotificationType.Transaction,
                NotificationSeverity.Success,
                JsonSerializer.Serialize(tx));
            await unitOfWork.WithTransactionAsync(async () =>
            {
                await notificationRepo.AddNotificationAsync(notification, ct);
            }, ct);
            await clientHubService.SendNotificationToMemberAsync(tx.ReceiverId, notification.Adapt<NotificationDto>());
        }
        
        string? userId = tx.Type == TransactionType.TransferReceived ? tx.ReceiverId : tx.TransferorId;
        if (userId != null)
            await clientHubService.SendTransactionTransferAsync(userId, tx);
    }
}