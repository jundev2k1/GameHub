namespace game_x.application.Features.LiveStreams.Commands.UnpublishStream;

public record UnpublishStreamCommand(string Server, string StreamKey) : ICommand;
