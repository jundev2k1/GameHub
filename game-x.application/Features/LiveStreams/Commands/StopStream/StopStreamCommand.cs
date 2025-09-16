namespace game_x.application.Features.LiveStreams.Commands.StopStream;

public record StopStreamCommand(
    string Server,
    string StreamKey,
    string Token) : ICommand;
