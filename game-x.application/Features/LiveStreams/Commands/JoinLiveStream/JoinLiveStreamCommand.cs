namespace game_x.application.Features.LiveStreams.Commands.JoinLiveStream;

public record JoinLiveStreamCommand(Guid Id) : ICommand<string>;
