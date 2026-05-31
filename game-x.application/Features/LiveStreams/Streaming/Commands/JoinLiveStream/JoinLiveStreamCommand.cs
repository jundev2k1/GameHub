using game_x.application.Features.LiveStreams.Streaming.Dtos;

namespace game_x.application.Features.LiveStreams.Streaming.Commands.JoinLiveStream;

public record JoinLiveStreamCommand(Guid Id) : ICommand<JoinLiveStreamResult>;

public record JoinLiveStreamResult(
    string Title,
    string? Description,
    string? Thumbnail,
    string StreamKey,
    DateTime LiveAt,
    bool IsLive,
    string TalentId,
    string TalentName,
    string? TalentAvatar,
    int ViewCount,
    string Url,
    string WebRtcUrl,
    LiveStreamBanInfoDto[] BanInfos);
