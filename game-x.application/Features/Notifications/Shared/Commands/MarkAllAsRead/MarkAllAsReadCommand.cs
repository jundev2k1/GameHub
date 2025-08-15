namespace game_x.application.Features.Notifications.Shared.Commands.MarkAllAsRead;

public record MarkAllAsReadCommand(string UserId) : ICommand;
