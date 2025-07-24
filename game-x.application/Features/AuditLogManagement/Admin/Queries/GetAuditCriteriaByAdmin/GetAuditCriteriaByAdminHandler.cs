using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.AuditLogManagement.Dtos;
using game_x.application.Mappers.BankAccount;

namespace game_x.application.Features.AuditLogManagement.Admin.Queries.GetAuditCriteriaByAdmin;

public sealed class GetAuditCriteriaByAdminHandler(
    ICriteriaBuilder<AuditLog> builder,
    AuditLogMapper auditMapper,
    IAuditLogRepo auditLogRepo) : IQueryHandler<GetAuditCriteriaByAdminQuery, PaginationResult<AuditLogDto>>
{
    public async Task<PaginationResult<AuditLogDto>> Handle(GetAuditCriteriaByAdminQuery request, CancellationToken ct = default)
    {
        var items = await auditLogRepo.GetByCriteriaAsync(
            query => builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword =>
                    audit => (audit.ChangedBy != null) && audit.ChangedBy.UserName!.StartsWith(keyword)),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);
        var result = auditMapper.ToSearchResult(items);
        return result;
    }
}
