namespace game_x.application.Contract.Persistence.Repo;

public interface IGameCategoryRepo
{
    Task<GameCategory[]> GetAllAsync(CancellationToken ct = default);

    Task AddAsync(GameCategory category, CancellationToken ct = default);

    Task UpdateAsync(Guid id, Func<GameCategory, Task> updateAction, CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
