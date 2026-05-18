using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.Account.OnUserBalanceUpdated;
using game_x.application.Features.Transactions.Dtos;
using game_x.share.Context;
using System.Text.Json;

namespace game_x.application.Events.Transactions.OnFailedTransaction;

public sealed class OnFailedTransactionHandler(
    IUnitOfWork unitOfWork,
    IClientHubService clientHubService,
    ITransactionRepo transactionRepo,
    IUserBalanceRepo userBalanceRepo,
    INotificationRepo notificationRepo,
    IAdminHubService adminHubService,
    IApplicationEventDispatcher eventDispatcher,
    IAppLogger<OnFailedTransactionHandler> logger) : IApplicationEventHandler<OnFailedTransactionEvent>
{
    public async Task Handle(OnFailedTransactionEvent @event, CancellationToken ct = default)
    {
        try
        {
            var targetTransaction = await transactionRepo.GetByOrderNumberAsync(@event.OrderNumber ?? string.Empty, ct)
                ?? throw new NotFoundException(
                    MessageCode.Transaction.TradeNotFound,
                    $"Transaction with order number '{@event.OrderNumber}' not found.");

            // Anti-spam request if the Transaction has already been updated
            if (targetTransaction.Status is TransactionStatus.Completed or TransactionStatus.Failed)
                throw new BadRequestException(MessageCode.System.InvalidCurrentStatus);

            var balance = targetTransaction.User.UserBalances.FirstOrDefault(b => b.CryptoTokenId == targetTransaction.CryptoTokenId)
                ?? throw new BadRequestException(MessageCode.Accounting.BalanceNotFound);

            var userId = targetTransaction.UserId;
            NotificationDto? notification = null;
            await unitOfWork.WithTransactionAsync(async () =>
            {
                switch (targetTransaction.Type)
                {
                    case TransactionType.Withdrawal:
                        balance.AdjustAmount(@event.Amount, true);
                        balance.FinalizeFrozen(@event.Amount);
                        break;

                    default:
                        throw new BadRequestException(MessageCode.System.InvalidParameters);
                }
                await userBalanceRepo.PutUpdateAsync(balance, ct);

                await transactionRepo.UpdateAsync(targetTransaction.PublicId, transaction =>
                {
                    transaction.MarkAsFailed(
                        @event.OrderUid,
                        @event.OrderNumber,
                        @event.FailureCategory,
                        @event.FailureCode,
                        @event.FailureMessage,
                        @event.FailedAt);
                }, ct);
                // Ensure the audit log records the order status updated by the external API
                using (AuditSourceContext.Use(AuditSource.External))
                {
                    await unitOfWork.SaveChangesAsync(ct);
                }
                var newTx = await transactionRepo.GetInternalByIdAsync(targetTransaction.PublicId, ct);
                notification = await SendNotificationAsync(newTx.Adapt<TransactionInternalDto>(), ct);
            }, ct);

            var newTx = await transactionRepo.GetInternalByIdAsync(targetTransaction.PublicId, ct);
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
            NotificationMessageKey.Transaction_Failed,
            transaction.UserId,
            NotificationType.Transaction,
            NotificationSeverity.Error,
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
