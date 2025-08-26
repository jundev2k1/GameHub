namespace game_x.application.Contract.Persistence.Repo;

public interface IGameTypeRepo
{
    Task<GameType[]> GetAllAsync(CancellationToken ct = default);
}
