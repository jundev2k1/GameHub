using game_x.application.Common.Filters;

namespace game_x.application.Features.LiveStreams.Queries.GetSchedulesByCriteria;

public record GetSchedulesByCriteriaQuery(
    IEnumerable<QueryFilter> Filters,
    IEnumerable<QuerySort> Sorts,
    int PageIndex = 1,
    int PageSize = 20) : IQuery<object>;
