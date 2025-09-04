using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using System.Linq.Expressions;

namespace game_x.persistence.Repo;

public sealed class GameRepo(GameXContext context) : IGameRepo, IRepository
{
    public async Task<Game[]> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Games
            .AsNoTracking()
            .Include(g => g.Platform)
            .Include(g => g.GameCategoryMappings)
            .ThenInclude(gcm => gcm.Category)
            .Include(g => g.GameTypeMappings)
            .ThenInclude(gtm => gtm.Type)
            .Include(g => g.GameTagMappings)
            .ThenInclude(gtm => gtm.Tag)
            .ToArrayAsync(ct);
    }

    public async Task<PaginationResult<Game>> GetsByCriteriaAsync(
        Expression<Func<Game, bool>> condition,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = context.Games
            .AsNoTracking()
            .Include(g => g.Platform)
            .Include(g => g.GameCategoryMappings)
            .ThenInclude(gcm => gcm.Category)
            .Include(g => g.GameTypeMappings)
            .ThenInclude(gtm => gtm.Type)
            .Include(g => g.GameTagMappings)
            .ThenInclude(gtm => gtm.Tag)
            .Where(condition)
            .OrderBy(g => g.Priority)
            .ThenBy(g => g.Name)
            .AsQueryable();

        var totalCount = await query.CountAsync(ct);
        var totalPageCount = (int)Math.Ceiling((decimal)totalCount / pageSize);
        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(ct);

        return new PaginationResult<Game>(
            result,
            totalCount,
            totalPageCount,
            page,
            pageSize);
    }
}
