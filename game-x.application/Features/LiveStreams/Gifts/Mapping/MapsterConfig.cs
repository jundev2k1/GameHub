using game_x.application.Features.LiveStreams.Gifts.Dtos;

namespace game_x.application.Features.LiveStreams.Gifts.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<LiveStreamGift, LiveStreamGiftDto>()
            .Map(dest => dest.Id, src => src.PublicId);

        cfg.NewConfig<LiveStreamGift, LiveStreamGiftDetailDto>()
            .Inherits<LiveStreamGift, LiveStreamGiftDto>()
            .Map(dest => dest.GiftPrices, src => src.GiftPrices.Select(gp => gp.Adapt<LiveStreamGiftPriceDto>()));

        cfg.NewConfig<LiveStreamGift, LiveStreamGiftClientDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.GiftPrices, src => src.GiftPrices.Select(gp => gp.Adapt<LiveStreamGiftPriceClientDto>()));

        cfg.NewConfig<LiveStreamGiftPrice, LiveStreamGiftPriceDto>()
            .Map(dest => dest.LiveStreamGiftLocalId, src => src.Id)
            .Map(dest => dest.LiveStreamGiftId, src => src.LiveStreamGift.PublicId)
            .Map(dest => dest.CryptoTokenLocalId, src => src.CryptoTokenId)
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken.PublicId);

        cfg.NewConfig<LiveStreamGiftPrice, LiveStreamGiftPriceClientDto>()
            .Map(dest => dest.CryptoTokenId, src => src.CryptoToken.PublicId);
    }
}
