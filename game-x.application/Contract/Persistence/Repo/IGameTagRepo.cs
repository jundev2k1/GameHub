namespace game_x.application.Contract.Persistence.Repo;

public interface IGameTagRepo
{
    Task<GameTag[]> GetAllAsync(CancellationToken ct = default);

    Task AddAsync(GameTag entity, CancellationToken ct = default);

    Task UpdateAsync(Guid id, Func<GameTag, Task> updateAction, CancellationToken ct = default);

    Task UpdateTranslationAsync(
        Guid gameId,
        Action<GameTag> updateAction,
        CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);
}