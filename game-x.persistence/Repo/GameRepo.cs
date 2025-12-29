using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

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
            .Include(g => g.Thumbnail)
            .ToArrayAsync(ct);
    }

    public async Task<Game> GetAsync(Guid gameId, CancellationToken ct = default)
    {
        return await context.Games
            .AsNoTracking()
            .Include(g => g.GameCategoryMappings)
            .ThenInclude(gcm => gcm.Category)
            .Include(g => g.GameTypeMappings)
            .ThenInclude(gtm => gtm.Type)
            .Include(g => g.GameTagMappings)
            .ThenInclude(gtm => gtm.Tag)
            .Include(g => g.Thumbnail)
            .FirstOrDefaultAsync(g => g.PublicId == gameId, ct)
            ?? throw new NotFoundException(nameof(gameId), gameId);
    }

    public async Task<PaginationResult<Game>> GetsByCriteriaAsync(
        Func<IQueryable<Game>, IQueryable<Game>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
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
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var totalPageCount = (int)Math.Ceiling((decimal)totalCount / pageSize);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(ct);

        return new PaginationResult<Game>(
            items,
            totalCount,
            totalPageCount,
            page,
            pageSize);
    }

    public async Task AddRangeGameCategoriesAsync(IEnumerable<GameCategoryMapping> gameCateMappings, CancellationToken ct = default)
    {
        await context.GameCategoryMappings.AddRangeAsync(gameCateMappings, ct);
    }

    public async Task AddRangeGameTypesAsync(IEnumerable<GameTypeMapping> gameTypeMappings, CancellationToken ct = default)
    {
        await context.GameTypeMappings.AddRangeAsync(gameTypeMappings, ct);
    }

    public async Task AddRangeGameTagsAsync(IEnumerable<GameTagMapping> gameTagMappings, CancellationToken ct = default)
    {
        await context.GameTagMappings.AddRangeAsync(gameTagMappings, ct);
    }

    public async Task UpdateGameAsync(
        Guid gameId,
        Func<Game, Task> updateAction,
        CancellationToken ct = default)
    {
        var targetGame = await context.Games
            .Include(g => g.GameCategoryMappings)
            .ThenInclude(gcm => gcm.Category)
            .Include(g => g.GameTypeMappings)
            .ThenInclude(gtm => gtm.Type)
            .Include(g => g.GameTagMappings)
            .ThenInclude(gtm => gtm.Tag)
            .Include(g => g.Thumbnail)
            .FirstOrDefaultAsync(g => g.PublicId == gameId, ct)
            ?? throw new NotFoundException(nameof(gameId), gameId);

        await updateAction.Invoke(targetGame);
    }

    public async Task DeleteAllCategoryMappingsAsync(Guid gameId, CancellationToken ct = default)
    {
        var categories = await context.GameCategoryMappings
            .Where(gcm => gcm.Game.PublicId == gameId)
            .ToArrayAsync(ct);
        context.RemoveRange(categories);
    }

    public async Task DeleteAllTypeMappingsAsync(Guid gameId, CancellationToken ct = default)
    {
        var types = await context.GameTypeMappings
            .Where(gtm => gtm.Game.PublicId == gameId)
            .ToArrayAsync(ct);
        context.RemoveRange(types);
    }

    public async Task DeleteAllTagMappingsAsync(Guid gameId, CancellationToken ct = default)
    {
        var tags = await context.GameTagMappings
            .Where(gtm => gtm.Game.PublicId == gameId)
            .ToArrayAsync(ct);
        context.RemoveRange(tags);
    }
}
