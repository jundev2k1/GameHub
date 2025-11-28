namespace game_x.application.Contract.Persistence.Repo;

public interface ISystemWalletRepo
{
    Task UpdateAsync(SystemWalletType type, Action<SystemWallet> updateAction, CancellationToken ct = default);
}
