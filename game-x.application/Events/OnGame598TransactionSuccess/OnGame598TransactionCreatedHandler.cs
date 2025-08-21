using System.Text.Json;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.OnGame598TransactionSuccess;

public sealed class OnGame598TransactionCreatedHandler(
    IUnitOfWork unitOfWork,
    INotificationRepo notificationRepo,
    IUserUsdtLedgerRepo userUsdtLedgerRepo,
    IClientHubService clientHubService) : IApplicationEventHandler<OnGame598TransactionCreatedEvent>
{
    public async Task Handle(OnGame598TransactionCreatedEvent @event, CancellationToken ct = default)
    {
        var transaction = @event.Transaction;
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await SendToMember(transaction, ct);
        }, ct);
    }

    private async Task SendToMember(GameTransaction transaction, CancellationToken ct)
    {
        UserBalance? balance = transaction.User.UserBalances.FirstOrDefault(b => b.CryptoTokenId == transaction.CryptoTokenId);
        if (balance != null)
        {
            CryptoToken token = balance.CryptoToken;
            var balanceNotification = new GameBalanceNotificationDto(
                Amount: balance.Amount,
                FrozenAmount: balance.FrozenAmount,
                Network: token.Network.ToString().ToLower(),
                Symbol: token.Symbol);

            UserUsdtLedger userLedger = await userUsdtLedgerRepo.GetDetailByGameTransactionIdAsync(transaction.Id);

            var notification = Notification.Create(
                NotificationMessageKey.Balance_Updated,
                balance.UserId,
                NotificationType.UserBalance,
                NotificationSeverity.Info,
                JsonSerializer.Serialize(balanceNotification));
            await notificationRepo.AddNotificationAsync(notification, ct);
            
            await clientHubService.SendNotificationToMemberAsync(
                balance.UserId,
                notification.Adapt<NotificationDto>());

            await clientHubService.SendLedgerToMemberAsync(
                balance.UserId,
                new ClientLedgerDto(
                    LedgerId: userLedger.PublicId,
                    Status: userLedger.StatusAtEvent));
        }
    }
}
