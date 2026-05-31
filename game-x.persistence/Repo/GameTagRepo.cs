using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class GameTagRepo(GameXContext context) : IGameTagRepo, IRepository
{
    public async Task<GameTag[]> GetAllAsync(CancellationToken ct = default)
    {
        return await context.GameTags
            .AsNoTracking()
            .Include(gt => gt.Translations)
            .ToArrayAsync(ct);
    }

    public async Task AddAsync(GameTag entity, CancellationToken ct = default)
    {
        await context.GameTags.AddAsync(entity, ct);
    }

    public async Task UpdateAsync(Guid id, Func<GameTag, Task> updateAction, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(updateAction);

        var targetTag = await context.GameTags
            .FirstOrDefaultAsync(t => t.PublicId == id && t.IsActive, ct)
            ?? throw new NotFoundException(nameof(id), id);

        await updateAction(targetTag);
    }

    public async Task UpdateTranslationAsync(
        Guid gameId,
        Action<GameTag> updateAction,
        CancellationToken ct = default)
    {
        var targetGame = await context.GameTags
            .Include(g => g.Translations)
            .FirstOrDefaultAsync(g => g.PublicId == gameId, ct)
            ?? throw new NotFoundException(nameof(gameId), gameId);

        updateAction.Invoke(targetGame);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var targetTag = await context.GameTags
            .FirstOrDefaultAsync(t => t.PublicId == id && t.IsActive, ct)
            ?? throw new NotFoundException(nameof(id), id);

        context.GameTags.Remove(targetTag);
    }
}