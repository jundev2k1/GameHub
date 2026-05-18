using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class NavigationItemRepo(GameXContext dbContext) : INavigationItemRepo, IRepository
{
    public async Task<NavigationItem[]> GetAllAsync(CancellationToken ct = default)
    {
        return await dbContext.NavigationItems
            .AsNoTracking()
            .AsSplitQuery()
            .Include(i => i.Icon)
            .Include(i => i.Translations)
            .ToArrayAsync(ct);
    }

    public async Task CreateAsync(NavigationItem item, CancellationToken ct = default)
    {
        await dbContext.NavigationItems.AddAsync(item, ct);
    }

    public async Task UpdateAsync(
        Guid id,
        Action<NavigationItem> updateAction,
        Func<IQueryable<NavigationItem>, IQueryable<NavigationItem>>? preUpdateAction = null,
        CancellationToken ct = default)
    {
        var query = dbContext.NavigationItems
            .AsQueryable();

        if (preUpdateAction != null)
            query = preUpdateAction(query);

        var target = await query.FirstOrDefaultAsync(i => i.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        updateAction?.Invoke(target);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var target = await dbContext.NavigationItems
            .FirstOrDefaultAsync(i => i.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        dbContext.Remove(target);
    }
}
