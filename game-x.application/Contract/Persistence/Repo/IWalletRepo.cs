namespace game_x.application.Contract.Persistence.Repo;

public interface IWalletRepo
{
    IQueryable<Wallet> Query();

    Task<Wallet?> GetByAddressAsync(string address, NetworkType network, CancellationToken ct = default);

    Task<Wallet?> GetByUserIdAndNetworkAsync(string userId, NetworkType network, CancellationToken ct = default);
}