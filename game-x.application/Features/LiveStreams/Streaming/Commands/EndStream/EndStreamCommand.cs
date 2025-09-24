namespace game_x.application.Features.LiveStreams.Streaming.Commands.EndStream;

public record EndStreamCommand(string StreamKey) : ICommand;
