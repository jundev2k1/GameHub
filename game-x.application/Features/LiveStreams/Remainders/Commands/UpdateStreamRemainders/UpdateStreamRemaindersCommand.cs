namespace game_x.application.Features.LiveStreams.Remainders.Commands.UpdateStreamRemainders;

public record UpdateStreamRemaindersCommand(
    [property: JsonIgnore] string? StreamKey,
    NotificationChannel[] Channels) : ICommand;
