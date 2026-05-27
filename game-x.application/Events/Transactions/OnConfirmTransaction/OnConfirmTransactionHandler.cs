using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.Account.OnUserBalanceUpdated;
using game_x.application.Features.Transactions.Dtos;
using game_x.share.Context;
using System.Text.Json;
using game_x.application.Contract.Infrastructure.BackgroundJobs.Dispatchers;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;

namespace game_x.application.Events.Transactions.OnConfirmTransaction;

public sealed class OnConfirmTransactionHandler(
    IUnitOfWork unitOfWork,
    IClientHubService clientHubService,
    ITransactionRepo transactionRepo,
    IUserBalanceRepo userBalanceRepo,
    INotificationRepo notificationRepo,
    IAdminHubService adminHubService,
    IApplicationEventDispatcher eventDispatcher,
    IUserEventJobDispatcher userEventDispatcher,
    IUserEventRepo userEventRepo,
    IAppLogger<OnConfirmTransactionHandler> logger) : IApplicationEventHandler<OnConfirmTransactionEvent>
{
    public async Task Handle(OnConfirmTransactionEvent @event, CancellationToken ct = default)
    {
        try
        {
            var transaction = await transactionRepo.GetByOrderNumberAsync(@event.OrderNumber ?? string.Empty, ct)
                ?? throw new NotFoundException(
                    MessageCode.Transaction.TradeNotFound,
                    $"Transaction with order number '{@event.OrderNumber}' not found.");

            // Anti-spam request if the Transaction has already been updated
            if (transaction.Status == TransactionStatus.Completed)
                throw new BadRequestException(MessageCode.System.InvalidCurrentStatus);

            var balance = transaction.User.UserBalances.FirstOrDefault(b => b.CryptoTokenId == transaction.CryptoTokenId)
                ?? throw new BadRequestException(MessageCode.Accounting.BalanceNotFound);

            var userId = transaction.UserId;
            NotificationDto? notification = null;
            var userEventId = Guid.CreateVersion7();
            await unitOfWork.WithTransactionAsync(async () =>
            {
                switch (transaction.Type)
                {
                    case TransactionType.Deposit:
                    {
                        balance.AdjustAmount(@event.ActualAmount, true);
                        var userEvent = UserEvent.Create(
                            userId: userId, 
                            type: UserEventType.DepositCompleted, 
                            id: userEventId,
                            value: @event.ActualAmount);
                    
                        await userEventRepo.AddAsync(userEvent, ct);
                        break;
                    }

                    case TransactionType.Withdrawal:
                        var amount = @event.ActualAmount + (transaction.Fee ?? 0);
                        balance.FinalizeFrozen(amount);
                        break;

                    default:
                        throw new BadRequestException(MessageCode.System.InvalidParameters);
                }
                await userBalanceRepo.PutUpdateAsync(balance, ct);

                await transactionRepo.UpdateAsync(transaction.PublicId, order =>
                {
                    order.UpdateStatus(TransactionStatus.Completed);
                    order.UpdateProviderResponse(
                        balanceAfter: balance.TotalAmount,
                        actualAmount: @event.ActualAmount,
                        providerOrderId: @event.ProviderOrderId,
                        hash: @event.Hash,
                        confirmedAt: @event.ConfirmedAt,
                        completedAt: DateTime.UtcNow);
                }, ct);
                
                // Ensure the audit log records the order status updated by the external API
                using (AuditSourceContext.Use(AuditSource.External))
                {
                    await unitOfWork.SaveChangesAsync(ct);
                }
                var newTx = await transactionRepo.GetInternalByIdAsync(transaction.PublicId, ct);
                
                if(transaction.Type ==  TransactionType.Deposit)
                    userEventDispatcher.EnqueueProcess(userEventId);
                
                notification = await SendNotificationAsync(newTx.Adapt<TransactionInternalDto>(), ct);
            }, ct);

            var newTx = await transactionRepo.GetInternalByIdAsync(transaction.PublicId, ct);
            var transactionInternal = newTx.Adapt<TransactionInternalDto>();
            await SendToMember(transactionInternal, notification!);
            await SendToAdmin(transactionInternal);

            await eventDispatcher.Publish(new OnUserBalanceUpdatedEvent(userId), ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }
    }

    private async Task<NotificationDto> SendNotificationAsync(TransactionInternalDto transaction, CancellationToken ct)
    {
        var notification = Notification.Create(
            NotificationMessageKey.Transaction_Completed,
            transaction.UserId,
            NotificationType.Transaction,
            NotificationSeverity.Success,
            JsonSerializer.Serialize(transaction.Adapt<TransactionNotificationDto>()));
        await notificationRepo.AddNotificationAsync(notification, ct);
        return notification.Adapt<NotificationDto>();
    }

    private async Task SendToMember(TransactionInternalDto transaction, NotificationDto notification)
    {
        var userId = transaction.UserId;
        await clientHubService.SendNotificationToMemberAsync(userId, notification);
        await clientHubService.SendTransactionToMemberAsync(userId, transaction.Adapt<ClientTransactionDto>());
    }

    private async Task SendToAdmin(TransactionInternalDto transaction)
    {
        await adminHubService.SendTransactionToAllAdminAsync(transaction.Adapt<AdminTransactionDto>());
    }
}