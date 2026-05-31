namespace game_x.application.Contract.Persistence.Repo;

public interface IGameMediaRepo
{
    Task<GameMedia[]> GetsByGameIdAsync(Guid gameId, CancellationToken ct = default);

    Task<GameMedia> GetAsync(Guid id, CancellationToken ct = default);

    Task CreateAsync(GameMedia gameMedia, CancellationToken ct = default);

    Task CreateRangeAsync(IEnumerable<GameMedia> gameMedias, CancellationToken ct = default);

    Task UpdateAsync(
        Guid id,
        Func<IQueryable<GameMedia>, IQueryable<GameMedia>>? preUpdateAction,
        Func<GameMedia, Task> updateAction,
        CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
