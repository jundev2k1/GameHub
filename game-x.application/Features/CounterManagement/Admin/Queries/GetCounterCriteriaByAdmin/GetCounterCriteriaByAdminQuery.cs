using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.CounterManagement.Dtos;

namespace game_x.application.Features.CounterManagement.Admin.Queries.GetCounterCriteriaByAdmin;

public record GetCounterCriteriaByAdminQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int? PageIndex,
    int? PageSize) : IQuery<PaginationResult<CounterDto>>;