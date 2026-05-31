using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetTypeByCriteria;

public sealed class GetTypeByCriteriaHandler(
    ICriteriaBuilder<GameTypeDto> builder,
    IGameProviderCacheService gameProviderCache) : IQueryHandler<GetTypeByCriteriaQuery, PaginationResult<GameTypeDto>>
{
    public async Task<PaginationResult<GameTypeDto>> Handle(GetTypeByCriteriaQuery request, CancellationToken ct = default)
    {
        var searchResult = builder.Apply(
            query: gameProviderCache.GameTypeList.AsQueryable(),
            filters: request.Filters,
            sorts: request.Sorts,
            keyword => dto => dto.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase));

        var totalItems = searchResult.Count();
        var items = searchResult
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToArray();

        var result = new PaginationResult<GameTypeDto>(
            items: items,
            totalItems: totalItems,
            totalPages: (int)Math.Ceiling((decimal)totalItems / request.PageSize),
            pageIndex: request.PageIndex,
            pageSize: request.PageSize);
        return await Task.FromResult(result);
    }
}
