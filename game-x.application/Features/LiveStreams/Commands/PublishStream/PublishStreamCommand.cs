namespace game_x.application.Features.LiveStreams.Commands.PublishStream;

public record PublishStreamCommand(string StreamKey, string Token) : ICommand;
