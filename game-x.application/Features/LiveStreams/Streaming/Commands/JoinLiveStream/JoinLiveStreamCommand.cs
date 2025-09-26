using game_x.application.Features.LiveStreams.Streaming.Dtos;
using game_x.application.Features.LiveStreams.Streaming.Enum;

namespace game_x.application.Features.LiveStreams.Streaming.Commands.JoinLiveStream;

public record JoinLiveStreamCommand(Guid Id) : ICommand<JoinLiveStreamResult>;

public record JoinLiveStreamResult(
    string Title,
    string? Description,
    string? Thumbnail,
    string StreamKey,
    DateTime LiveAt,
    string TalentId,
    string TalentName,
    string? TalentAvatar,
    int ViewCount,
    string Url,
    ViewerBanInfo[] BanInfos);

public record ViewerBanInfo(
    BlackListAction Action,
    DateTime BlockTo,
    BlockReasonEnum Reason);
