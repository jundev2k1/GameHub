using System.Text.Json;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Services.UserUsdtLedger;
using game_x.application.Contract.Infrastructure.Services.Wallet;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUserBalanceUpdated;
using game_x.share.Context;

namespace game_x.application.Events.OnUxmTransactionCallback;

public sealed class OnUxmTransactionCallbackHandler(
    IUnitOfWork unitOfWork,
    IClientHubService clientHubService,
    IUserBalanceService userBalanceService,
    IChainTransactionRepo chainTransactionRepo,
    IUserBalanceRepo userBalanceRepo,
    IUserUsdtLedgerService userUsdtLedgerService,
    IUserRepo userRepo,
    INotificationRepo notificationRepo,
    IAdminHubService adminHubService,
    IApplicationEventDispatcher eventDispatcher,
    IAppLogger<ChainTransaction> logger) : IApplicationEventHandler<OnUxmTransactionCallbackEvent>
{
    public async Task Handle(OnUxmTransactionCallbackEvent @event, CancellationToken ct = default)
    {
        try
        {
            ChainTransaction? transaction = 
                await chainTransactionRepo.GetByOrderNumberAsync(@event.OrderNumber ?? string.Empty, ct);
            
            if(transaction == null)
                throw new NotFoundException(MessageCode.Transaction.TradeNotFound,
                    $"Transaction with order number '{@event.OrderNumber}' not found.");
                
            // Anti-spam request if the Transaction has already been updated
            if (transaction.Status == ChainTransactionStatus.Completed)
                throw new BadRequestException(MessageCode.System.InvalidCurrentStatus);

            UserBalance? balance = transaction.User?.UserBalances.FirstOrDefault(b => b.CryptoTokenId == transaction.CryptoTokenId);
            if (balance == null)
                throw new BadRequestException(MessageCode.Accounting.BalanceNotFound);
            
            await unitOfWork.WithTransactionAsync(async () =>
            {
                await chainTransactionRepo.PatchUpdateAsync(transaction.PublicId, order =>
                {
                    order.UpdateStatus(ChainTransactionStatus.Completed);
                    order.UpdateUxmResponse(
                        actualAmount: @event.ActualAmount,
                        orderUid: @event.OrderUid ?? string.Empty,
                        hash: @event.Hash ?? string.Empty,
                        confirmedAt: @event.ConfirmedAt
                    );
                }, ct);
                
                // Create transaction history before updating balance
                await userUsdtLedgerService.CreateForChainTransactionAsync(transaction);
                
                switch (transaction.Type)
                {
                    case ChainTransactionType.Deposit:
                        userBalanceService.IncreaseAmount(balance, @event.ActualAmount);
                        break;
                    case ChainTransactionType.Withdrawal:
                        userBalanceService.FinalizeFrozen(balance, @event.ActualAmount);
                        break;
                    default:
                        throw new BadRequestException(MessageCode.System.InvalidParameters);
                }
                await userBalanceRepo.PutUpdateAsync(balance, ct);
                
                await SendToMember(transaction, ct);
                await SendToAdmin(transaction, ct);
            }, ct);
            
            // Ensure the audit log records the order status updated by the external API
            using (AuditSourceContext.Use(AuditSource.External))
            {
                await unitOfWork.SaveChangesAsync(ct);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }
    }

    private async Task SendToMember(ChainTransaction transaction, CancellationToken ct)
    {
        var userId = transaction.UserId;
        if (userId != null)
        {
            var notification = Notification.Create(
                NotificationMessageKey.Transaction_Completed,
                userId,
                NotificationType.Transaction,
                NotificationSeverity.Success,
                JsonSerializer.Serialize(transaction.Adapt<TransactionNotificationDto>()));
            
            await notificationRepo.AddNotificationAsync(notification, ct);
                
            await clientHubService.SendNotificationToMemberAsync(
                userId,
                notification.Adapt<NotificationDto>());
            
            await clientHubService.SendTransactionToMemberAsync(
                userId,
                transaction.Adapt<ClientTransactionDto>());
            
            await eventDispatcher.Publish(new OnUserBalanceUpdatedEvent(userId), ct);
        }
    }
    
    private async Task SendToAdmin(ChainTransaction transaction, CancellationToken ct)
    {
        var adminUsers = await userRepo.GetAdminUsers(ct);
        var metadata = JsonSerializer.Serialize(transaction.Adapt<TransactionNotificationDto>());
        foreach (var adminUser in adminUsers)
        { 
            var notification = Notification.Create(
                NotificationMessageKey.Transaction_Completed,
                adminUser.Id,
                NotificationType.Transaction,
                NotificationSeverity.Success,
                metadata);
            await notificationRepo.AddNotificationAsync(notification, ct);

            await adminHubService.SendNotificationAsync(
                adminUser.Id,
                notification.Adapt<NotificationDto>());

            await adminHubService.SendTransactionToAdminAsync(
                adminUser.Id,
                new AdminTransactionDto(
                    TransactionId: transaction.PublicId,
                    Status: transaction.Status.ToString(),
                    Type: transaction.Type.ToString()));
        }
    }
}