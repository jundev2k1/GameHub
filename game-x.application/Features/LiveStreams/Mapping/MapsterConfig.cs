using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Features.LiveStreams.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<LivestreamSchedule, LiveStreamScheduleListItemDto>()
            .Map(dest => dest.Id, src => src.PublicId);

        cfg.NewConfig<LivestreamSchedule, LiveStreamScheduleDto>()
            .Map(dest => dest.Id, src => src.PublicId);

        cfg.NewConfig<LiveStreamCategory, LiveStreamCategoryListItemDto>()
            .Map(dest => dest.Id, src => src.PublicId);

        cfg.NewConfig<LiveStreamCategory, LiveStreamCategoryDto>()
            .Map(dest => dest.Id, src => src.PublicId);
    }
}
