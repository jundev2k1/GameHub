using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Features.LiveStreams.Queries.GetScheduleDetail;

public record GetScheduleDetailQuery(Guid Id) : IQuery<LiveStreamScheduleDto>;
