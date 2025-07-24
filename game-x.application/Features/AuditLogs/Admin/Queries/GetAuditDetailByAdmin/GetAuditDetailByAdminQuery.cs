using game_x.application.Features.AuditLogs.Dtos;

namespace game_x.application.Features.AuditLogs.Admin.Queries.GetAuditDetailByAdmin;

public record GetAuditDetailByAdminQuery(Guid Id) : IQuery<AuditLogDto>;
