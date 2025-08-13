using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.domain.Constants;

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
    
    public async Task<CryptoToken> GetByIdAsync(Guid cryptoTokenId, CancellationToken ct = default)
    {
        return await context.CryptoTokens.AsNoTracking()
            .FirstOrDefaultAsync(t => t.PublicId == cryptoTokenId, ct)
               ?? throw new BadRequestException(MessageCode.Crypto.CryptoTokenNotFound);
    }

    public async Task<CryptoToken?> GetBySymbolAsync(string symbol, CancellationToken ct = default)
    {
        return await context.CryptoTokens.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Symbol == symbol, ct);
    }
}