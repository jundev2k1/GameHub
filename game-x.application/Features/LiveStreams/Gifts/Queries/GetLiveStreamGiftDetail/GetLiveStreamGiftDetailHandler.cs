using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Gifts.Dtos;

namespace game_x.application.Features.LiveStreams.Gifts.Queries.GetLiveStreamGiftDetail;

public sealed class GetLiveStreamGiftDetailHandler(ILiveStreamGiftRepo liveStreamGiftRepo)
    : IQueryHandler<GetLiveStreamGiftDetailQuery, LiveStreamGiftDetailDto>
{
    public async Task<LiveStreamGiftDetailDto> Handle(GetLiveStreamGiftDetailQuery request, CancellationToken ct = default)
    {
        var result = await liveStreamGiftRepo.GetDetailByIdAsync(request.GiftId, ct);
        return result;
    }
}
