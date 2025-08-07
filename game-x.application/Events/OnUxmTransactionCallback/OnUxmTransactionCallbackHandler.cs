using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.OnUxmTransactionCallback;

public sealed class OnUxmTransactionCallbackHandler(
    IUnitOfWork unitOfWork,
    IClientHubService clientHubService) : IApplicationEventHandler<OnUxmTransactionCallbackEvent>
{
    public async Task Handle(OnUxmTransactionCallbackEvent @event, CancellationToken ct = default)
    {
        var targetTransaction = @event.Transaction;
        await unitOfWork.WithTransactionAsync(async () => { await SendToMember(targetTransaction, ct); }, ct);
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