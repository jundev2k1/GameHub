using System.Text.Json;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Services.Wallet;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUserBalanceUpdated;
using game_x.application.Features.ChainTransactions.Dtos;
using game_x.share.Context;

namespace game_x.application.Events.OnUxmTransactionCallback;

public sealed class OnUxmTransactionCallbackHandler(
    IUnitOfWork unitOfWork,
    IClientHubService clientHubService,
    IUserBalanceService userBalanceService,
    ITransactionRepo transactionRepo,
    IUserBalanceRepo userBalanceRepo,
    IUserRepo userRepo,
    INotificationRepo notificationRepo,
    IAdminHubService adminHubService,
    IApplicationEventDispatcher eventDispatcher,
    IAppLogger<Transaction> logger) : IApplicationEventHandler<OnUxmTransactionCallbackEvent>
{
    public async Task Handle(OnUxmTransactionCallbackEvent @event, CancellationToken ct = default)
    {
        try
        {
            Transaction? transaction = await transactionRepo.GetByOrderNumberAsync(@event.OrderNumber ?? string.Empty, ct);

            if (transaction == null)
                throw new NotFoundException(MessageCode.Transaction.TradeNotFound,
                    $"Transaction with order number '{@event.OrderNumber}' not found.");

            // Anti-spam request if the Transaction has already been updated
            if (transaction.Status == TransactionStatus.Completed)
                throw new BadRequestException(MessageCode.System.InvalidCurrentStatus);

            UserBalance? balance = transaction.User.UserBalances.FirstOrDefault(b => b.CryptoTokenId == transaction.CryptoTokenId);
            if (balance == null)
                throw new BadRequestException(MessageCode.Accounting.BalanceNotFound);

            await unitOfWork.WithTransactionAsync(async () =>
            {
                switch (transaction.Type)
                {
                    case TransactionType.Deposit:
                        userBalanceService.IncreaseAmount(balance, @event.ActualAmount);
                        break;

                    case TransactionType.Withdrawal:
                        userBalanceService.FinalizeFrozen(balance, @event.ActualAmount + (transaction.Fee ?? 0));
                        break;

                    default:
                        throw new BadRequestException(MessageCode.System.InvalidParameters);
                }
                await userBalanceRepo.PutUpdateAsync(balance, ct);
                
                await transactionRepo.PatchUpdateAsync(transaction.PublicId, order =>
                {
                    order.UpdateStatus(TransactionStatus.Completed);
                    order.UpdateProviderResponse(
                        actualAmount: @event.ActualAmount,
                        providerOrderId: @event.ProviderOrderId,
                        hash: @event.Hash,
                        confirmedAt: @event.ConfirmedAt,
                        completedAt: DateTime.UtcNow
                    );
                    
                    order.BalanceAfter = balance.TotalAmount;
                }, ct);
                
                var transactionInternal = transaction.Adapt<TransactionInternalDto>();
                await SendToMember(transactionInternal, ct);
                await SendToAdmin(transactionInternal, ct);
            }, ct);

            // Ensure the audit log records the order status updated by the external API
            using (AuditSourceContext.Use(AuditSource.External))
            {
                await unitOfWork.SaveChangesAsync(ct);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }
    }

    private async Task SendToMember(TransactionInternalDto transaction, CancellationToken ct)
    {
        var userId = transaction.UserId;
        var notification = Notification.Create(
            NotificationMessageKey.Transaction_Completed,
            userId,
            NotificationType.Transaction,
            NotificationSeverity.Success,
            JsonSerializer.Serialize(transaction.Adapt<TransactionNotificationDto>()));
        await notificationRepo.AddNotificationAsync(notification, ct);
        await clientHubService.SendNotificationToMemberAsync(userId, notification.Adapt<NotificationDto>());
        await clientHubService.SendTransactionToMemberAsync(userId, transaction.Adapt<ClientTransactionDto>());
        await eventDispatcher.Publish(new OnUserBalanceUpdatedEvent(userId), ct);
    }

    private async Task SendToAdmin(TransactionInternalDto transaction, CancellationToken ct)
    {
        var adminUsers = await userRepo.GetAdminUsers(ct);
        foreach (var adminUser in adminUsers)
        {
            await adminHubService.SendTransactionToAdminAsync(adminUser.Id, transaction.Adapt<AdminTransactionDto>());
        }
    }
}