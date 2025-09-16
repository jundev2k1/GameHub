using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetRecommendsByCriteria;

public sealed class GetRecommendsByCriteriaHandler(
    ICriteriaBuilder<GameRecommendDto> builder,
    IGameProviderCacheService gameProviderCache) : IQueryHandler<GetRecommendsByCriteriaQuery, PaginationResult<GetRecommendByCriteriaListItem>>
{
    public async Task<PaginationResult<GetRecommendByCriteriaListItem>> Handle(GetRecommendsByCriteriaQuery request, CancellationToken ct = default)
    {
        var searchResult = builder.Apply(
            query: gameProviderCache.GameRecommendList.AsQueryable(),
            filters: request.Filters,
            sorts: request.Sorts,
            keyword => dto => dto.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase));

        var totalItems = searchResult.Count();
        var items = searchResult
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(i => i.Adapt<GetRecommendByCriteriaListItem>())
            .ToArray();

        var result = new PaginationResult<GetRecommendByCriteriaListItem>(
            items: items,
            totalItems: totalItems,
            totalPages: (int)Math.Ceiling((decimal)totalItems / request.PageSize),
            pageIndex: request.PageIndex,
            pageSize: request.PageSize);
        return await Task.FromResult(result);
    }
}
