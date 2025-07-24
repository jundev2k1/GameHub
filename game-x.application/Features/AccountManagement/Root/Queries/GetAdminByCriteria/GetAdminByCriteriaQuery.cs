using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.AccountManagement.Dtos;

namespace game_x.application.Features.AccountManagement.Root.Queries.GetAdminByCriteria;

public record GetAdminByCriteriaQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int? PageIndex,
    int? PageSize) : IQuery<PaginationResult<AdminDto>>;