using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetCategoriesByCriteria;

public sealed class GetCategoriesByCriteriaHandler(
    ICriteriaBuilder<GameCategoryDto> builder,
    IGameProviderCacheService gameProviderCache) : IQueryHandler<GetCategoriesByCriteriaQuery, PaginationResult<GameCategoryDto>>
{
    public async Task<PaginationResult<GameCategoryDto>> Handle(GetCategoriesByCriteriaQuery request, CancellationToken ct = default)
    {
        var searchResult = builder.Apply(
            query: gameProviderCache.CategoryList.AsQueryable(),
            filters: request.Filters,
            sorts: request.Sorts,
            keyword => dto => dto.Name.Contains(keyword, StringComparison.InvariantCultureIgnoreCase));

        var totalItems = searchResult.Count();
        var items = searchResult
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToArray();

        var result = new PaginationResult<GameCategoryDto>(
            items: items,
            totalItems: totalItems,
            totalPages: (int)Math.Ceiling((decimal)totalItems / request.PageSize),
            pageIndex: request.PageIndex,
            pageSize: request.PageSize);
        return await Task.FromResult(result);
    }
}