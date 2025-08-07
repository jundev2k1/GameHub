using System.Text.Json;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Events.OnTransactionCreated;

public sealed class OnTransactionCreatedHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    INotificationRepo notificationRepo,
    IAdminHubService adminHubService,
    IClientHubService clientHubService) : IApplicationEventHandler<OnTransactionCreatedEvent>
{
    public async Task Handle(OnTransactionCreatedEvent @event, CancellationToken ct = default)
    {
        var targetTransaction = @event.Transaction;
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await SendToMember(targetTransaction, ct);
            await SendToAdmin(targetTransaction, ct);
        }, ct);
    }

    private async Task SendToAdmin(ChainTransaction transaction, CancellationToken ct)
    {
        var adminUsers = await userRepo.GetAdminUsers(ct);

        foreach (var adminUser in adminUsers)
        {
            var notification = Notification.Create(
                NotificationMessageKey.Transaction_Created,
                adminUser.Id,
                NotificationType.Transaction,
                NotificationSeverity.Success,
                JsonSerializer.Serialize(transaction.Adapt<TransactionNotificationDto>()));
            await notificationRepo.AddNotificationAsync(notification, ct);

            // Send notification to all the admin
            await adminHubService.SendNotificationAsync(
                adminUser.Id,
                notification.Adapt<NotificationDto>());

            // Send transaction to all the admin
            await adminHubService.SendTransactionToAdminAsync(
                adminUser.Id,
                new AdminTransactionDto(
                    TransactionId: transaction.PublicId,
                    Status: transaction.Status.ToString(),
                    Type: transaction.Type.ToString()));
        }
    }
    
    private async Task SendToMember(ChainTransaction transaction, CancellationToken ct)
    {
        if (transaction.UserId != null)
        {
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