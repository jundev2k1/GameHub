using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class GameMediaRepo(GameXContext dbContext) : IGameMediaRepo, IRepository
{
    public async Task<GameMedia[]> GetsByGameIdAsync(Guid gameId, CancellationToken ct = default)
    {
        return await dbContext.GameMedias
            .AsNoTracking()
            .Include(gm => gm.Game)
            .Where(gm => gm.Game.PublicId == gameId)
            .ToArrayAsync(ct);
    }

    public async Task<GameMedia> GetAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.GameMedias
            .AsNoTracking()
            .Include(gm => gm.Game)
            .FirstOrDefaultAsync(gm => gm.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);
    }

    public async Task CreateAsync(GameMedia gameMedia, CancellationToken ct = default)
    {
        await dbContext.GameMedias.AddAsync(gameMedia, ct);
    }

    public async Task CreateRangeAsync(IEnumerable<GameMedia> gameMedias, CancellationToken ct = default)
    {
        await dbContext.GameMedias.AddRangeAsync(gameMedias, ct);
    }

    public async Task UpdateAsync(
        Guid id,
        Func<IQueryable<GameMedia>, IQueryable<GameMedia>>? preUpdateAction,
        Func<GameMedia, Task> updateAction,
        CancellationToken ct = default)
    {
        var query = dbContext.GameMedias
            .AsSplitQuery()
            .AsQueryable();
        if (preUpdateAction != null)
            query = preUpdateAction(query);

        var target = await query
            .FirstOrDefaultAsync(q => q.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        await updateAction.Invoke(target);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var target = await dbContext.GameMedias
            .FirstOrDefaultAsync(gm => gm.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        dbContext.Remove(target);
    }
}
