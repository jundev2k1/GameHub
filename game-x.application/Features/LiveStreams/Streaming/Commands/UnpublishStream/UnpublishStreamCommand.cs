namespace game_x.application.Features.LiveStreams.Streaming.Commands.UnpublishStream;

public record UnpublishStreamCommand(string Server, string StreamKey) : ICommand;
