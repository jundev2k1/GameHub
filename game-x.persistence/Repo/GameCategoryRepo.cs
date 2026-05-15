using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class GameCategoryRepo(GameXContext context)
    : IGameCategoryRepo, IRepository
{
    public async Task<GameCategory[]> GetAllAsync(CancellationToken ct = default)
    {
        return await context.GameCategories
            .AsNoTracking()
            .Include(c => c.Translations)
            .ToArrayAsync(ct);
    }

    public async Task AddAsync(GameCategory category, CancellationToken ct = default)
    {
        await context.GameCategories.AddAsync(category, ct);
    }

    public async Task UpdateAsync(Guid id, Func<GameCategory, Task> updateAction, CancellationToken ct = default)
    {
        var targetCategory = await context.GameCategories
            .FirstOrDefaultAsync(c => c.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        await updateAction.Invoke(targetCategory);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var targetCategory = await context.GameCategories
            .FirstOrDefaultAsync(c => c.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        context.GameCategories.Remove(targetCategory);
    }
}
