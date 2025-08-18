using System.Text.Json;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.OnGame598TransactionSuccess;

public sealed class OnGame598TransactionCreatedHandler(
    IUnitOfWork unitOfWork,
    INotificationRepo notificationRepo,
    IClientHubService clientHubService) : IApplicationEventHandler<OnGame598TransactionCreatedEvent>
{
    public async Task Handle(OnGame598TransactionCreatedEvent @event, CancellationToken ct = default)
    {
        var targetTransaction = @event.Transaction;
        var token = @event.Token;
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await SendToMember(targetTransaction, token, ct);

        }, ct);
    }

    private async Task SendToMember(GameTransaction transaction, CryptoToken token, CancellationToken ct)
    {
        if (transaction.UserId != null)
        {

            UserBalance? balance = transaction.User?.UserBalances.FirstOrDefault(b => b.CryptoTokenId == transaction.CryptoTokenId);

            if (balance != null)
            {
                var balanceDto = new GameBalanceDto(
                Amount: balance.Amount,
                FrozenAmount: balance.FrozenAmount,
                Network: token.Network);

                var notification = Notification.Create(
                    NotificationMessageKey.Balance_Updated,
                    balance.UserId,
                    NotificationType.UserBalance,
                    NotificationSeverity.Info,
                    JsonSerializer.Serialize(balanceDto));
                await notificationRepo.AddNotificationAsync(notification, ct);

                await clientHubService.SendGameBalanceToMemberAsync(
                    transaction.UserId,
                    balanceDto);
            }
        }
    }
}
