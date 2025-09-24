using game_x.application.Features.LiveStreams.Schedules.Dtos;

namespace game_x.application.Features.LiveStreams.Schedules.Queries.GetScheduleDetail;

public record GetScheduleDetailQuery(Guid Id) : IQuery<GetScheduleDetailResult>;

public class GetScheduleDetailResult : LiveStreamScheduleDto
{
    public string StreamUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
}
