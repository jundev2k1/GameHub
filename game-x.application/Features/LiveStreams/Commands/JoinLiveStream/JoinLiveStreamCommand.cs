namespace game_x.application.Features.LiveStreams.Commands.JoinLiveStream;

public record JoinLiveStreamCommand(Guid Id) : ICommand<JoinLiveStreamResult>;

public record JoinLiveStreamResult(
    string Title,
    string? Description,
    string? Thumbnail,
    DateTime LiveAt,
    string TalentId,
    string TalentName,
    int ViewerCount,
    string Url);
