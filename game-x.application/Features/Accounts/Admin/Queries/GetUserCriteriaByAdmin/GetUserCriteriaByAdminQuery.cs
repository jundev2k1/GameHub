using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.Accounts.Dtos;

namespace game_x.application.Features.Accounts.Admin.Queries.GetUserCriteriaByAdmin;

public record GetUserCriteriaByAdminQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int? PageIndex,
    int? PageSize) : IQuery<PaginationResult<UserListItemDto>>;