using System.Text.Json;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUserBalanceUpdated;
using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Events.OnWithdrawalOrderReviewed;

public sealed class OnWithdrawalOrderReviewedHandler(
    IUnitOfWork unitOfWork,
    INotificationRepo notificationRepo,
    IClientHubService clientHubService,
    IApplicationEventDispatcher eventDispatcher,
    IAppLogger<User> logger) : IApplicationEventHandler<OnWithdrawalOrderReviewedEvent>
{
    public async Task Handle(OnWithdrawalOrderReviewedEvent @event, CancellationToken ct = default)
    {
        var targetTransaction = @event.Transaction;
        await unitOfWork.WithTransactionAsync(async () => { await SendToMember(targetTransaction, ct); }, ct);
    }

    private async Task SendToMember(TransactionInternalDto transaction, CancellationToken ct)
    {
        try
        {
            var notification = Notification.Create(
                NotificationMessageKey.Transaction_Reviewed,
                transaction.UserId,
                NotificationType.Transaction,
                NotificationSeverity.Success,
                JsonSerializer.Serialize(transaction.Adapt<TransactionNotificationDto>()));
            await notificationRepo.AddNotificationAsync(notification, ct);

            await clientHubService.SendNotificationToMemberAsync(
                transaction.UserId,
                notification.Adapt<NotificationDto>());
            
            await clientHubService.SendTransactionToMemberAsync(
                transaction.UserId,
                transaction.Adapt<ClientTransactionDto>());
            
            await eventDispatcher.Publish(new OnUserBalanceUpdatedEvent(transaction.UserId), ct);
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to envoke signals to members", ex.Message);
        }
    }
}
