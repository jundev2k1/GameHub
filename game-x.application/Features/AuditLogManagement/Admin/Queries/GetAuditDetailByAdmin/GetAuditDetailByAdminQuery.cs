using game_x.application.Features.AuditLogManagement.Dtos;

namespace game_x.application.Features.AuditLogManagement.Admin.Queries.GetAuditDetailByAdmin;

public record GetAuditDetailByAdminQuery(Guid Id) : IQuery<AuditLogDto>;
