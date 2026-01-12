using System.Text.Json.Serialization;

namespace game_x.application.Features.LiveStreams.Remainders.Commands.SubscribeStream;

public record SubscribeStreamCommand(
    [property: JsonIgnore] string? StreamKey,
    NotificationChannel[] Channels) : ICommand;
