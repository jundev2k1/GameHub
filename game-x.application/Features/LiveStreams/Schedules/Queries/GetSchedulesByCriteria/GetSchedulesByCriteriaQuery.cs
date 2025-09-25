using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Features.LiveStreams.Schedules.Dtos;

namespace game_x.application.Features.LiveStreams.Schedules.Queries.GetSchedulesByCriteria;

public record GetSchedulesByCriteriaQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int PageIndex = 1,
    int PageSize = 20) : IQuery<PaginationResult<LiveStreamScheduleListItemDto>>;
