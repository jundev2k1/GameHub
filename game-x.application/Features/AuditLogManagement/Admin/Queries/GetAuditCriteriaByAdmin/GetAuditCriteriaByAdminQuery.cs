using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.AuditLogManagement.Dtos;

namespace game_x.application.Features.AuditLogManagement.Admin.Queries.GetAuditCriteriaByAdmin;

public record GetAuditCriteriaByAdminQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int? PageIndex,
    int? PageSize) : IQuery<PaginationResult<AuditLogDto>>;
