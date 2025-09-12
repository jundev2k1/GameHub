namespace game_x.application.Features.LiveStreams.Commands.PublishStream;

public record PublishStreamCommand(string Server, string StreamKey) : ICommand;
