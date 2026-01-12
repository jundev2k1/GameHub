using System.Text.Json.Serialization;

namespace game_x.application.Features.LiveStreams.Reminders.Commands.UpdateStreamReminders;

public record UpdateStreamRemindersCommand(
    [property: JsonIgnore] string? StreamKey,
    NotificationChannel[] Channels) : ICommand;
