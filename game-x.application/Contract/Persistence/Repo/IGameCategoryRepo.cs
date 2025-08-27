namespace game_x.application.Contract.Persistence.Repo;

public interface IGameCategoryRepo
{
    Task<GameCategory[]> GetAllAsync(CancellationToken ct = default);
}
