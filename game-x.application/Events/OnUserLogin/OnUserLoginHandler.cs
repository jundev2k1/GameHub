using System.Collections.Concurrent;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Events.OnUserLogin;

public sealed class OnUserLoginHandler(
    ICryptoTokenRepo cryptoTokenRepo,
    IUserUsdtLedgerRepo userUsdtLedgerRepo,
    IUserRepo userRepo,
    IUserBalanceRepo userBalanceRepo,
    IAppLogger<ChainTransaction> logger): IApplicationEventHandler<OnUserLoginEvent>
{
    private static readonly ConcurrentDictionary<string, byte> ProcessingUsers = new();
    
    public async Task Handle(OnUserLoginEvent @event, CancellationToken ct)
    {
        var userId = @event.UserId;
        if (!ProcessingUsers.TryAdd(userId, 0)) return;
        
        try
        {
            var user = await userRepo.GetUserByIdAsync(userId, ct);
            await EnsureInitLedgerAsync(user, ct);
        }
        finally
        {
            // await Task.Delay(1000, ct);
            ProcessingUsers.TryRemove(userId, out _);
        }
    }
    
    private async Task EnsureInitLedgerAsync(User user, CancellationToken ct)
    {
        try
        {
            var hasLedger = await userUsdtLedgerRepo.GetLatestLedgerAsync(user.Id);
            if (hasLedger != null) return;

            var token = await cryptoTokenRepo.GetBySymbolAndNetworkAsync(CryptoTokenSymbol.Usdt, NetworkType.Tron, ct);
            if (token == null) return;

            // Get the user's current USDT balance
            var usdtBalance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(user.Id, token.Id, ct);
            if (usdtBalance == null) return;

            var initLedger = new UserUsdtLedger
            {
                UserId = user.Id,
                Timestamp = DateTime.UtcNow,
                FlowType = UsdtFlowType.Init,
                SourceId = $"init-{Guid.NewGuid()}",
                ChangeAmount = 0m,
                BalanceAfter = usdtBalance.TotalAmount,
                MetaObject = new UserUsdtLedgerMeta
                {
                    CounterpartyName = "System"
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await userUsdtLedgerRepo.AddAsync(initLedger, ct);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }
    }
}