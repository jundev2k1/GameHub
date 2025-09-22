using game_x.application.Features.LiveStreams.Enum;

namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.LiveStream;

public sealed class LiveStreamBanInfo
{
    public required PerformActionEnum Action { get; set; }
    public DateTime? BanUntil { get; set; }
    public BlockReasonEnum? Reason { get; set; }
}
