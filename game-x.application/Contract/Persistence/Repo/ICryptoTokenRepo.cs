namespace game_x.application.Contract.Persistence.Repo;

public interface ICryptoTokenRepo
{
    Task<IReadOnlyList<CryptoToken>> GetAsync();

    Task<IEnumerable<CryptoToken>> GetCryptoTokenListAsync(CancellationToken ct = default);

    Task<CryptoToken?> GetBySymbolAndNetworkAsync(string symbol, NetworkType network, CancellationToken ct = default);

    Task<CryptoToken[]> GetByIdsAsync(Guid[] ids, CancellationToken ct = default);

    Task<CryptoToken> GetByIdAsync(Guid cryptoTokenId, CancellationToken ct = default);
}