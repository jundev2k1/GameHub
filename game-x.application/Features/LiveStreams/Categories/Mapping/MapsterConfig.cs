using game_x.application.Features.LiveStreams.Categories.Dtos;

namespace game_x.application.Features.LiveStreams.Categories.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<LiveStreamCategory, LiveStreamCategoryListItemDto>()
            .Map(dest => dest.Id, src => src.PublicId);

        cfg.NewConfig<LiveStreamCategory, LiveStreamCategoryDto>()
            .Map(dest => dest.Id, src => src.PublicId);
    }
}
