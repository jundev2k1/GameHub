using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class GameTypeRepo(GameXContext context)
    : IGameTypeRepo, IRepository
{
    public async Task<GameType[]> GetAllAsync(CancellationToken ct = default)
    {
        return await context.GameTypes
            .AsNoTracking()
            .Include(gt => gt.Translations)
            .ToArrayAsync(ct);
    }

    public async Task AddAsync(GameType gameType, CancellationToken ct = default)
    {
        await context.GameTypes.AddAsync(gameType, ct);
    }

    public async Task UpdateAsync(Guid id, Func<GameType, Task> updateAction, CancellationToken ct = default)
    {
        var targetType = await context.GameTypes
            .FirstOrDefaultAsync(gt => gt.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        await updateAction.Invoke(targetType);
    }

    public async Task UpdateTranslationAsync(
        Guid gameId,
        Action<GameType> updateAction,
        CancellationToken ct = default)
    {
        var targetGame = await context.GameTypes
            .Include(g => g.Translations)
            .FirstOrDefaultAsync(g => g.PublicId == gameId, ct)
            ?? throw new NotFoundException(nameof(gameId), gameId);

        updateAction.Invoke(targetGame);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var targetType = await context.GameTypes
            .FirstOrDefaultAsync(gt => gt.PublicId == id)
            ?? throw new NotFoundException(nameof(id), id);

        context.GameTypes.Remove(targetType);
    }
}
