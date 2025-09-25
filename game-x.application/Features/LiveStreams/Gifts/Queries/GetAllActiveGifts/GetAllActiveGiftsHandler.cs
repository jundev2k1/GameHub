using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Features.LiveStreams.Gifts.Dtos;

namespace game_x.application.Features.LiveStreams.Gifts.Queries.GetAllActiveGifts;

public sealed class GetAllActiveGiftsHandler(
    ILiveStreamManagerCacheService liveStreamManager) : IQueryHandler<GetAllActiveGiftsQuery, LiveStreamGiftClientDto[]>
{
    public async Task<LiveStreamGiftClientDto[]> Handle(GetAllActiveGiftsQuery request, CancellationToken ct = default)
    {
        var result = await liveStreamManager.GetAllActiveGiftsAsync(ct);
        return result;
    }
}
