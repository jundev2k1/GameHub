using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Services.UserUsdtLedger;
using game_x.application.Contract.Infrastructure.Services.Wallet;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.Context;

namespace game_x.application.Events.OnUxmTransactionCallback;

public sealed class OnUxmTransactionCallbackHandler(
    IUnitOfWork unitOfWork,
    IClientHubService clientHubService,
    IUserBalanceService userBalanceService,
    IChainTransactionRepo chainTransactionRepo,
    IUserBalanceRepo userBalanceRepo,
    IUserUsdtLedgerService userUsdtLedgerService,
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
                
                switch (transaction.Type)
                {
                    case ChainTransactionType.Deposit:
                        userBalanceService.AddAmount(balance, @event.ActualAmount);
                        break;
                    case ChainTransactionType.Withdrawal:
                        userBalanceService.FinalizeFrozen(balance, @event.ActualAmount);
                        break;
                    default:
                        throw new BadRequestException(MessageCode.System.InvalidParameters);
                }
                await userBalanceRepo.PutUpdateAsync(balance, ct);
                
                await userUsdtLedgerService.CreateForChainTransactionAsync(transaction);
                
                if(transaction.UserId != null)
                    await SendToMember(transaction.UserId, balance, ct);
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

    private async Task SendToMember(string userId, UserBalance balance, CancellationToken ct)
    {
        await clientHubService.SendBalanceToMemberAsync(
            userId,
            new ClientBalanceDto(
                BalanceId: balance.PublicId,
                Amount: balance.Amount,
                FrozenAmount: balance.FrozenAmount));
    }
}