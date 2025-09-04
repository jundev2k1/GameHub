using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetGames;

public sealed class GetGamesHandler(
    IGameProviderCacheService gameProviderCache) : IQueryHandler<GetGamesQuery, PaginationResult<GameInfoDto>>
{
    public async Task<PaginationResult<GameInfoDto>> Handle(GetGamesQuery request, CancellationToken ct = default)
    {
        var items = gameProviderCache.GameList
            .Where(GetFilterCondition(request))
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToArray();
        var totalItems = gameProviderCache.GameList.Count(GetFilterCondition(request));
        var result = new PaginationResult<GameInfoDto>(
            items: items,
            totalItems: totalItems,
            totalPages: (int)Math.Ceiling((decimal)totalItems / request.PageSize),
            pageIndex: request.PageIndex,
            pageSize: request.PageSize);
        return await Task.FromResult(result);
    }

    private static Func<GameInfoDto, bool> GetFilterCondition(GetGamesQuery request)
    {
        return game =>
            ((request.IsActive == null)
                || (game.IsActive == request.IsActive))
            && ((request.PlatformId == null)
                || (game.PlatformId == request.PlatformId))
            && ((request.CategoryIds == null)
                || game.Categories.Any(c => request.CategoryIds.Contains(c.Id)))
            && ((request.TypeIds == null)
                || game.GameTypes.Any(t => request.TypeIds.Contains(t.Id)))
            && ((request.Keyword == null)
                || game.Id.ToString().Equals(request.Keyword.Trim(), StringComparison.InvariantCultureIgnoreCase)
                || game.Name.Contains(request.Keyword.Trim(), StringComparison.InvariantCultureIgnoreCase));
    }
}
