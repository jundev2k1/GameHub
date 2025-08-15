using System.Text.Json;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.OnUserBankAccountChanged;

public sealed class OnUserBankAccountChangedHandler(
    IUnitOfWork unitOfWork,
    INotificationRepo notificationRepo,
    IClientHubService clientHubService) : IApplicationEventHandler<OnUserBankAccountChangedEvent>
{
    public async Task Handle(OnUserBankAccountChangedEvent @event, CancellationToken ct = default)
    {
        var targetBalance = @event.UserBankAccount;
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await SendToMember(targetBalance, ct);
        }, ct);
    }

    private async Task SendToMember(UserBankAccount bankAccount, CancellationToken ct)
    {
        var notification = Notification.Create(
            NotificationMessageKey.Balance_Updated,
            bankAccount.UserId,
            NotificationType.UserBalance,
            NotificationSeverity.Info,
            JsonSerializer.Serialize(bankAccount.Adapt<ClientBalanceDto>()));
        await notificationRepo.AddNotificationAsync(notification, ct);

        await clientHubService.SendBalanceToMemberAsync(
            bankAccount.UserId,
            new ClientBalanceDto(
                BalanceId: bankAccount.PublicId,
                Amount: bankAccount.Amount,
                FrozenAmount: bankAccount.FrozenAmount));
    }
}