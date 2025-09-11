using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using System.Linq.Expressions;

namespace game_x.persistence.Repo;

public sealed class LiveStreamCategoryRepo(GameXContext context) : ILiveStreamCategoryRepo, IRepository
{
    public async Task<LiveStreamCategory[]> GetAllAsync(CancellationToken ct)
    {
        var result = await context.LiveStreamCategories
            .AsNoTracking()
            .ToArrayAsync(ct);
        return result;
    }

    public async Task<LiveStreamCategory[]> GetByIdsAsync(Guid[] ids, CancellationToken ct = default)
    {
        var result = await context.LiveStreamCategories
            .AsNoTracking()
            .Where(lsc => ids.Contains(lsc.PublicId) && lsc.IsActive)
            .ToArrayAsync(ct);
        return result;
    }

    public async Task<PaginationResult<LiveStreamCategory>> GetsByCriteriaAsync(
        Expression<Func<LiveStreamCategory, bool>> condition,
        int page,
        int pageSize, CancellationToken ct)
    {
        var query = context.LiveStreamCategories
            .AsNoTracking()
            .Where(condition)
            .AsQueryable();

        var totalCount = await query.CountAsync(ct);
        var totalPageCount = (int)Math.Ceiling((decimal)totalCount / pageSize);
        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(ct);

        return new PaginationResult<LiveStreamCategory>(
            result,
            totalCount,
            totalPageCount,
            page,
            pageSize);
    }

    public async Task CreateAsync(LiveStreamCategory category, CancellationToken ct)
    {
        await context.LiveStreamCategories.AddAsync(category, ct);
    }

    public async Task UpdateAsync(Guid categoryId, Func<LiveStreamCategory, Task> updateAction, CancellationToken ct)
    {
        var targetCategory = await context.LiveStreamCategories
            .FirstOrDefaultAsync(lsc => lsc.PublicId == categoryId, ct)
            ?? throw new NotFoundException(nameof(categoryId), categoryId);

        await updateAction.Invoke(targetCategory);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var targetCategory = await context.LiveStreamCategories
            .FirstOrDefaultAsync(lsc => lsc.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        context.LiveStreamCategories.Remove(targetCategory);
    }
}
