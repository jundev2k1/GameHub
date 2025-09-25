using game_x.application.Features.LiveStreams.Gifts.Dtos;

namespace game_x.application.Features.LiveStreams.Gifts.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<LiveStreamGift, LiveStreamGiftDto>()
            .Map(dest => dest.Id, src => src.PublicId);

        cfg.NewConfig<LiveStreamGift, LiveStreamGiftDetailDto>()
            .Inherits<LiveStreamGift, LiveStreamGiftDto>();

        cfg.NewConfig<LiveStreamGift, LiveStreamGiftClientDto>()
            .Map(dest => dest.Id, src => src.PublicId);
    }
}
