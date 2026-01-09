namespace game_x.application.Contract.Persistence.Repo;

public interface IS2sClientRepo
{
    Task<S2SClient[]> GetAllAsync(CancellationToken ct = default);

    Task<bool> IsExistAsync(string clientId, CancellationToken ct = default);

    Task CreateAsync(S2SClient entity, CancellationToken ct = default);

    Task UpdateAsync(string clientId, Action<S2SClient> updateAction, CancellationToken ct = default);

    Task DeleteAsync(string clientId, CancellationToken ct = default);
}
