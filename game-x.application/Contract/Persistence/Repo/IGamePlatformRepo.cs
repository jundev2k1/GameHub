namespace game_x.application.Contract.Persistence.Repo;

public interface IGamePlatformRepo
{
    Task<GamePlatform[]> GetAllAsync(CancellationToken ct = default);

    Task UpdateAsync(Guid id, Func<GamePlatform, Task> updateAction, CancellationToken ct = default);

    Task UpdateTranslationAsync(
        Guid gameId,
        Action<GamePlatform> updateAction,
        CancellationToken ct = default);
}
