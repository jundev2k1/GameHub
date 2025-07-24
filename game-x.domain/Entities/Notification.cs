namespace game_x.domain.Entities;

public sealed class Notification : BaseEntity<int>
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public NotificationMessageKey MessageKey { get; private set; }
    public string? UserId { get; private set; }
    public NotificationType Type { get; private set; }
    public bool IsRead { get; private set; } = false;
    public NotificationSeverity Severity { get; private set; } = NotificationSeverity.Info;
    public DateTime? ReadAt { get; private set; }
    public DateTime? ExpiredAt { get; private set; }
    public string? Metadata { get; private set; }

    public static Notification Create(
        NotificationMessageKey messageKey,
        string? userId,
        NotificationType type,
        NotificationSeverity severity,
        string? metaData = null,
        bool isRead = false,
        DateTime? readAt = null,
        DateTime? expiredAt = null)
    {
        if (userId is not null && userId.IsNullOrEmpty())
            throw new ArgumentException($"{nameof(userId)} must be not empty (but can null if this is broadcast).");

        if (!isRead && (readAt != null))
            throw new ArgumentException($"{nameof(readAt)} cannot be set if {nameof(isRead)} is false.");
        if (isRead && readAt is null)
            throw new ArgumentException($"{nameof(readAt)} must be set if {nameof(isRead)} is true.");

        if ((metaData != null) && JsonHelper.IsJson(metaData))
            ArgumentException.ThrowIfNullOrWhiteSpace(metaData, nameof(metaData));

        return new Notification
        {
            MessageKey = messageKey,
            UserId = userId,
            Type = type,
            Severity = severity,
            Metadata = metaData,
            IsRead = isRead,
            ReadAt = readAt,
            ExpiredAt = expiredAt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
    }

    public void MarkAsRead()
    {
        IsRead = true;
        ReadAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
