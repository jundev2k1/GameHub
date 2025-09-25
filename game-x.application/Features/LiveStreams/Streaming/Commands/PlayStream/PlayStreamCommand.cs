namespace game_x.application.Features.LiveStreams.Streaming.Commands.PlayStream;

public record PlayStreamCommand(
    string Server,
    string StreamKey,
    string Token,
    string ClientId) : ICommand;
