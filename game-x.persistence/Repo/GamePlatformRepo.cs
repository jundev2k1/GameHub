using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class GamePlatformRepo(GameXContext context)
    : IGamePlatformRepo, IRepository
{
    public async Task<GamePlatform[]> GetAllAsync(CancellationToken ct = default)
    {
        return await context.GamePlatforms
            .AsNoTracking()
            .Include(gp => gp.Translations)
            .ToArrayAsync(ct);
    }

    public async Task UpdateAsync(Guid id, Func<GamePlatform, Task> updateAction, CancellationToken ct = default)
    {
        var targetPlatform = await context.GamePlatforms
            .FirstOrDefaultAsync(gp => gp.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        await updateAction.Invoke(targetPlatform);
    }

    public async Task UpdateTranslationAsync(
        Guid gameId,
        Action<GamePlatform> updateAction,
        CancellationToken ct = default)
    {
        var targetGame = await context.GamePlatforms
            .Include(g => g.Translations)
            .FirstOrDefaultAsync(g => g.PublicId == gameId, ct)
            ?? throw new NotFoundException(nameof(gameId), gameId);

        updateAction.Invoke(targetGame);
    }
}
