using System.Text.Json;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Transactions.Dtos;

namespace game_x.application.Events.Transaction.OnTransactionTransferred;

public sealed class OnTransactionTransferredHandler(
    IUnitOfWork unitOfWork,
    INotificationRepo notificationRepo,
    IClientHubService clientHubService) : IApplicationEventHandler<OnTransactionTransferredEvent>
{
    public async Task Handle(OnTransactionTransferredEvent @event, CancellationToken ct = default)
    {
        await SendToMember(@event.TxDto, @event.SignalDto, ct);
    }

    private async Task SendToMember(TransactionTransferDto txDto, TransactionTransferSignalDto txData, CancellationToken ct)
    {
        if (txDto is {Type: TransactionType.TransferReceived, ReceiverId: not null})
        {
            var notification = Notification.Create(
                NotificationMessageKey.Transaction_Received,
                txDto.ReceiverId,
                NotificationType.Transaction,
                NotificationSeverity.Success,
                JsonSerializer.Serialize(txData));
            await unitOfWork.WithTransactionAsync(async () =>
            {
                await notificationRepo.AddNotificationAsync(notification, ct);
            }, ct);
            await clientHubService.SendNotificationToMemberAsync(txDto.ReceiverId, notification.Adapt<NotificationDto>());
        }
        
        string? userId = txDto.Type == TransactionType.TransferReceived ? txDto.ReceiverId : txDto.TransferorId;
        if (userId != null) await clientHubService.SendTransactionTransferAsync(userId, txData);
    }
}