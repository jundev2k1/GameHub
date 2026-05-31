namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;

public record NotificationDto(
    Guid NotificationId,
    string MessageKey,
    string Type,
    string Severity,
    string? Metadata,
    bool IsRead,
    bool IsBroadcast,
    DateTime CreatedAt);