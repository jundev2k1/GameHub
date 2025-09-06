using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Extensions.FilterExtensions;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetGamesByCriteria;

public sealed class GetGamesByCriteriaHandler(
    ICriteriaBuilder<GameInfoDto> builder,
    IGameProviderCacheService gameProviderCache) : IQueryHandler<GetGamesByCriteriaQuery, PaginationResult<GetGamesByCriteriaListItem>>
{
    public async Task<PaginationResult<GetGamesByCriteriaListItem>> Handle(GetGamesByCriteriaQuery request, CancellationToken ct = default)
    {
        var searchResult = builder.Apply(
            query: gameProviderCache.GameList.AsQueryable(),
            filters: request.Filters,
            sorts: request.Sorts,
            keyword => dto => dto.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase),
            GameFilterExtensions.Options);

        var totalItems = searchResult.Count();
        var items = searchResult
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(item => item.Adapt<GetGamesByCriteriaListItem>())
            .ToArray();

        var result = new PaginationResult<GetGamesByCriteriaListItem>(
            items: items,
            totalItems: totalItems,
            totalPages: (int)Math.Ceiling((decimal)totalItems / request.PageSize),
            pageIndex: request.PageIndex,
            pageSize: request.PageSize);
        return await Task.FromResult(result);
    }
}
