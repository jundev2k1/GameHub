using System.Text.Json;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Events.OnUserBalanceChanged;

public sealed class OnUserBalanceChangedHandler(
    IUnitOfWork unitOfWork,
    INotificationRepo notificationRepo,
    IClientHubService clientHubService) : IApplicationEventHandler<OnUserBalanceChangedEvent>
{
    public async Task Handle(OnUserBalanceChangedEvent @event, CancellationToken ct = default)
    {
        var targetBalance = @event.UserBalance;
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await SendToMember(targetBalance, ct);
        }, ct);
    }

    private async Task SendToMember(UserBalance balance, CancellationToken ct)
    {
        var notification = Notification.Create(
            NotificationMessageKey.Balance_Updated,
            balance.UserId,
            NotificationType.UserBalance,
            NotificationSeverity.Info,
            JsonSerializer.Serialize(balance.Adapt<ClientBalanceDto>()));
        await notificationRepo.AddNotificationAsync(notification, ct);

        await clientHubService.SendBalanceToMemberAsync(
            balance.UserId,
            new ClientBalanceDto(
                BalanceId: balance.PublicId,
                Amount: balance.Amount,
                FrozenAmount: balance.FrozenAmount));
    }
}