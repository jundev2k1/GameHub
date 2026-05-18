namespace game_x.application.Contract.Persistence.Repo;

public interface IGameTypeRepo
{
    Task<GameType[]> GetAllAsync(CancellationToken ct = default);

    Task AddAsync(GameType gameType, CancellationToken ct = default);

    Task UpdateAsync(Guid id, Func<GameType, Task> updateAction, CancellationToken ct = default);

    Task UpdateTranslationAsync(
        Guid gameId,
        Action<GameType> updateAction,
        CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
