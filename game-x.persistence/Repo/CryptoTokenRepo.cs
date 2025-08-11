using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.persistence.Repo;

public sealed class CryptoTokenRepo(GameXContext context): ICryptoTokenRepo, IRepository
{
    public async Task<IReadOnlyList<CryptoToken>> GetAsync()
    {
        return await context.CryptoTokens.AsNoTracking().ToListAsync();
    }
    
    public IQueryable<CryptoToken> Query()
    {
        return context.CryptoTokens;
    }

    public async Task<IEnumerable<CryptoToken>> GetCryptoTokenListAsync(CancellationToken ct = default)
    {
        return await context.CryptoTokens
            .AsNoTracking()
            .ToListAsync(ct);
    }
    
    public async Task<CryptoToken?> GetBySymbolAndNetworkAsync(string symbol, NetworkType network, CancellationToken ct = default)
    {
        return await context.CryptoTokens.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Symbol == symbol && t.Network == network, ct);
    }

    public async Task<CryptoToken?> GetBySymbolAsync(string symbol, CancellationToken ct = default)
    {
        return await context.CryptoTokens.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Symbol == symbol, ct);
    }
}