using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.LiveStreams.Schedules.Dtos;

namespace game_x.application.Features.LiveStreams.Streaming.Queries.GetActiveStreams;

public record GetActiveStreamsQuery(
    string? Keyword,
    DateTime? StartTime,
    DateTime? EndTime,
    int PageIndex = 1,
    int PageSize = 20) : IQuery<PaginationResult<LiveStreamScheduleClientItemDto>>;
