namespace game_x.application.Features.LiveStreams.Streaming.Commands.PublishStream;

public record PublishStreamCommand(string StreamKey, string Token, string ClientId) : ICommand;
