using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.persistence.Repo;

public sealed class GameCategoryRepo(GameXContext context)
    : IGameCategoryRepo, IRepository
{
    public async Task<GameCategory[]> GetAllAsync(CancellationToken ct = default)
    {
        return await context.GameCategories.ToArrayAsync(ct);
    }
}
