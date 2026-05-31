namespace game_x.application.Features.Notifications.Shared.Commands.MarkAsRead;

public record MarkAsReadCommand(Guid NotificationId, string UserId) : ICommand;
