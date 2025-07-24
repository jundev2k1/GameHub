namespace game_x.application.Features.Notification.Shared.Commands.MarkAsRead;

public record MarkAsReadCommand(Guid NotificationId, string UserId) : ICommand;
