namespace game_x.application.Contract.Persistence.Repo;

public interface ICryptoTokenRepo
{
    Task<IReadOnlyList<CryptoToken>> GetAsync();
    IQueryable<CryptoToken> Query();
    Task<IEnumerable<CryptoToken>> GetCryptoTokenListAsync(CancellationToken ct = default);
    Task<CryptoToken?> GetBySymbolAndNetworkAsync(string symbol, NetworkType network, CancellationToken ct = default);

    Task<CryptoToken?> GetBySymbolAsync(string symbol, CancellationToken ct);
}