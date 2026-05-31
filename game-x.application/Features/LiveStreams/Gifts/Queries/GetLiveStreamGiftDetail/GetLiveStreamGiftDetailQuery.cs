using game_x.application.Features.LiveStreams.Gifts.Dtos;

namespace game_x.application.Features.LiveStreams.Gifts.Queries.GetLiveStreamGiftDetail;

public record GetLiveStreamGiftDetailQuery(Guid GiftId) : IQuery<LiveStreamGiftDetailDto>;
