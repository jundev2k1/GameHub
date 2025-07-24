using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.AccountManagement.Dtos;

namespace game_x.application.Features.AccountManagement.Admin.Queries.GetStaffCriteriaByAdmin;

public record GetStaffCriteriaByAdminQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int? PageIndex,
    int? PageSize) : IQuery<PaginationResult<StaffDto>>;