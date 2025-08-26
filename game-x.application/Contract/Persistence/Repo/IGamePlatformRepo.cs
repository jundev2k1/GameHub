namespace game_x.application.Contract.Persistence.Repo;

public interface IGamePlatformRepo
{
    Task<GamePlatform[]> GetAllAsync(CancellationToken ct = default);
}
