using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.persistence.Repo;

public sealed class GameTypeRepo(GameXContext context)
    : IGameTypeRepo, IRepository
{
    public async Task<GameType[]> GetAllAsync(CancellationToken ct = default)
    {
        return await context.GameTypes.ToArrayAsync(ct);
    }
}
