using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.AuditLogManagement.Dtos;

namespace game_x.application.Features.AuditLogManagement.Admin.Queries.GetAuditDetailByAdmin;

public sealed class GetAuditDetailByAdminHandler(IAuditLogRepo auditLogRepo) : IQueryHandler<GetAuditDetailByAdminQuery, AuditLogDto>
{
    public async Task<AuditLogDto> Handle(GetAuditDetailByAdminQuery request, CancellationToken ct = default)
    {
        var targetAuditLog = await auditLogRepo.GetByIdAsync(request.Id, ct);
        var result = targetAuditLog.Adapt<AuditLogDto>();
        return result;
    }
}
