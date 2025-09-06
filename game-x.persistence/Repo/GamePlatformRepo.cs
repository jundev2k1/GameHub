using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class GamePlatformRepo(GameXContext context)
    : IGamePlatformRepo, IRepository
{
    public async Task<GamePlatform[]> GetAllAsync(CancellationToken ct = default)
    {
        return await context.GamePlatforms.ToArrayAsync(ct);
    }

    public async Task UpdateAsync(Guid id, Func<GamePlatform, Task> updateAction, CancellationToken ct = default)
    {
        var targetPlatform = await context.GamePlatforms
            .FirstOrDefaultAsync(gp => gp.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        await updateAction.Invoke(targetPlatform);
    }
}
