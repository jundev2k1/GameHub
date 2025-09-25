using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.LiveStreams.Categories.Dtos;

namespace game_x.application.Features.LiveStreams.Categories.Queries.GetCategoriesByCriteria;

public record GetCategoriesByCriteriaQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int PageIndex = 1,
    int PageSize = 20) : IQuery<PaginationResult<LiveStreamCategoryListItemDto>>;
