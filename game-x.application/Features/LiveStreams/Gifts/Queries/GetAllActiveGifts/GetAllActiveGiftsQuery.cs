using game_x.application.Features.LiveStreams.Gifts.Dtos;

namespace game_x.application.Features.LiveStreams.Gifts.Queries.GetAllActiveGifts;

public record GetAllActiveGiftsQuery : IQuery<LiveStreamGiftClientDto[]>;
