using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.Games.Dtos;

namespace game_x.application.Features.Games.Admin.Queries.GetGameTagsByCriteria;

public record GetGameTagsByCriteriaQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int PageIndex = 1,
    int PageSize = 20) : IQuery<PaginationResult<GameTagDto>>;
