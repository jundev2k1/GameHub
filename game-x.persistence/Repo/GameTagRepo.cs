using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.persistence.Repo;

public sealed class GameTagRepo(GameXContext context) : IGameTagRepo, IRepository
{
    public async Task<GameTag[]> GetAllAsync(CancellationToken ct = default)
    {
        return await context.GameTags.AsNoTracking().ToArrayAsync(ct);
    }
}
