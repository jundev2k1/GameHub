using game_x.application.Features.Accounts.Dtos;
using game_x.application.Features.LiveStreams.Dtos;
using game_x.application.Features.LiveStreams.Queries.GetScheduleDetail;

namespace game_x.application.Features.LiveStreams.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<LivestreamSchedule, LiveStreamScheduleListItemDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.AssignedTo, src => src.AssignedTo.Adapt<UserSummaryInfo>());

        cfg.NewConfig<LivestreamSchedule, LiveStreamScheduleDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.AssignedTo, src => src.AssignedTo.Adapt<UserSummaryInfo>());

        cfg.NewConfig<LivestreamSchedule, GetScheduleDetailResult>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.AssignedTo, src => src.AssignedTo.Adapt<UserSummaryInfo>());

        cfg.NewConfig<LiveStreamCategory, LiveStreamCategoryListItemDto>()
            .Map(dest => dest.Id, src => src.PublicId);

        cfg.NewConfig<LiveStreamCategory, LiveStreamCategoryDto>()
            .Map(dest => dest.Id, src => src.PublicId);
    }
}
