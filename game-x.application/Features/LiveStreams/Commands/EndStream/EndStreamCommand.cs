namespace game_x.application.Features.LiveStreams.Commands.EndStream;

public record EndStreamCommand(string StreamKey) : ICommand;
