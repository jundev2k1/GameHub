namespace game_x.application.Contract.Persistence.Repo;

public interface ITalentWalletRepo
{
    Task UpdateAsync(string userId, Action<TalentWallet> updateAction, CancellationToken ct = default);
}
