namespace game_x.domain.Entities;

public sealed class AuditLog : BaseEntity<int>
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public EntityName EntityName { get; private set; } = default!;
    public string EntityId { get; private set; } = string.Empty;
    public AuditAction Action { get; private set; }
    public string? ChangedById { get; private set; }
    public User? ChangedBy { get; private set; } = default!;
    public AuditSource Source { get; private set; } = default!;
    public string? Changes { get; private set; }
    public string? SnapshotBefore { get; private set; }
    public string? SnapshotAfter { get; private set; }

    public static AuditLog Create(
        EntityName entityName,
        string entityId,
        AuditAction action,
        string? changedByUserId,
        AuditSource source,
        string? changes = null,
        string? snapshotBefore = null,
        string? snapshotAfter = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entityId, nameof(entityId));
        if (changes.IsNotNullOrEmpty() && !JsonHelper.IsJsonArray(changes!))
            throw new ArgumentException("Changes must be an array json type.");
        if (snapshotBefore.IsNotNullOrEmpty() && !JsonHelper.IsJson(snapshotBefore!))
            throw new ArgumentException("Snapshot Before must be a json type.");
        if (snapshotAfter.IsNotNullOrEmpty() && !JsonHelper.IsJson(snapshotAfter!))
            throw new ArgumentException("Snapshot After must be a json type.");

        return new AuditLog
        {
            EntityName = entityName,
            EntityId = entityId,
            Action = action,
            ChangedById = changedByUserId,
            Source = source,
            Changes = changes,
            SnapshotBefore = snapshotBefore,
            SnapshotAfter = snapshotAfter,
            CreatedAt = DateTime.UtcNow,
        };
    }
}
