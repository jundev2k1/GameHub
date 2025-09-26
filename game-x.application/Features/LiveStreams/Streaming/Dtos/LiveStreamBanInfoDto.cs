using game_x.application.Features.LiveStreams.Streaming.Enum;

namespace game_x.application.Features.LiveStreams.Streaming.Dtos;

public record LiveStreamBanInfoDto(
    BlackListAction Action,
    DateTime BanUntil,
    BlockReasonEnum Reason);
