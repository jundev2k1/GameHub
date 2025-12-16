using game_x.application.Features.LiveStreams.Streaming.Dtos;

namespace game_x.application.Events.OnLiveStreamDonated;

public record OnLiveStreamDonatedEvent(
    LiveStreamStatusDto StreamInfo,
    string UserId,
    Guid UserBalanceId,
    decimal Amount,
    int CryptoId,
    string Message = "",
    LiveStreamGift? Gift = null) : IApplicationEvent;
