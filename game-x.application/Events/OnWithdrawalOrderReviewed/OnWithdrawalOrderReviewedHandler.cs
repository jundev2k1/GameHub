using System.Text.Json;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Events.OnWithdrawalOrderReviewed;

public sealed class OnWithdrawalOrderReviewedHandler(
    IUnitOfWork unitOfWork,
    INotificationRepo notificationRepo,
    IClientHubService clientHubService) : IApplicationEventHandler<OnWithdrawalOrderReviewedEvent>
{
    public async Task Handle(OnWithdrawalOrderReviewedEvent @event, CancellationToken ct = default)
    {
        var targetTransaction = @event.Transaction;
        await unitOfWork.WithTransactionAsync(async () => { await SendToMember(targetTransaction, ct); }, ct);
    }

    private async Task SendToMember(ChainTransaction transaction, CancellationToken ct)
    {
        var notification = Notification.Create(
            NotificationMessageKey.Transaction_Reviewed,
            transaction.UserId,
            NotificationType.Transaction,
            NotificationSeverity.Success,
            JsonSerializer.Serialize(transaction.Adapt<TransactionNotificationDto>()));
        await notificationRepo.AddNotificationAsync(notification, ct);

        if (transaction.UserId != null)
        {
            await clientHubService.SendNotificationToMemberAsync(
                transaction.UserId,
                notification.Adapt<NotificationDto>());
            
            await clientHubService.SendTransactionToMemberAsync(
                transaction.UserId,
                new ClientTransactionDto(
                    TransactionId: transaction.PublicId,
                    Status: transaction.Status.ToString().ToLower(),
                    Type: transaction.Type.ToString().ToLower()));
            
            UserBalance? balance = transaction.User?.UserBalances.FirstOrDefault(b => b.CryptoTokenId == transaction.CryptoTokenId);
            if (balance != null)
            {
                await clientHubService.SendBalanceToMemberAsync(
                    transaction.UserId,
                    new ClientBalanceDto(
                        BalanceId: balance.PublicId,
                        Amount: balance.Amount,
                        FrozenAmount: balance.FrozenAmount));
            }
        }
    }
}
