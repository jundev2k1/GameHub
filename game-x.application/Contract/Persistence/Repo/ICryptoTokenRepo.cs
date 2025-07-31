namespace game_x.application.Contract.Persistence.Repo;

public interface ICryptoTokenRepo
{
    IQueryable<CryptoToken> Query();

    Task<CryptoToken?> GetBySymbolAndNetworkAsync(string symbol, NetworkType network, CancellationToken ct = default);

    Task<CryptoToken?> GetBySymbolAsync(string symbol, CancellationToken ct);
}