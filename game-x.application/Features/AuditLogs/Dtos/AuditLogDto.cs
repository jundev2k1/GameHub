namespace game_x.application.Features.AuditLogs.Dtos;

public record AuditLogDto(
    Guid Id,
    string EntityName,
    string EntityId,
    string Action,
    string ChangedByUserId,
    string ChangedByUserName,
    string Source,
    string Changes,
    string SnapshotBefore,
    string SnapshotAfter,
    DateTime CreatedAt);
