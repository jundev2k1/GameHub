using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.persistence.Repo;

public sealed class GamePlatformRepo(GameXContext context)
    : IGamePlatformRepo, IRepository
{
    public async Task<GamePlatform[]> GetAllAsync(CancellationToken ct = default)
    {
        return await context.GamePlatforms.ToArrayAsync(ct);
    }
}
