namespace game_x.application.Contract.Persistence.Repo;

public interface IGameTagRepo
{
    Task<GameTag[]> GetAllAsync(CancellationToken ct = default);
}
