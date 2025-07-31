using game_x.application.Contract.Persistence.Repo;

namespace game_x.persistence.Repo;

public sealed class WalletRepo(GameXContext context): IWalletRepo
{
    public IQueryable<Wallet> Query()
    {
        return context.Wallets;
    }

    public async Task<Wallet?> GetByAddressAsync(string address, NetworkType network, CancellationToken ct = default)
    {
        return await context.Wallets
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.WalletAddress == address && w.Network == network, ct);
    }

    public async Task<Wallet?> GetByUserIdAndNetworkAsync(string userId, NetworkType network, CancellationToken ct = default)
    {
        return await context.Wallets
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.UserId == userId && w.Network == network, ct);
    }
}