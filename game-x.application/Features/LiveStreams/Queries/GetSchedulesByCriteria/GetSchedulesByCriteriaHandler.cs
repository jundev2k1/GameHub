using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Games.Mapping;
using game_x.application.Features.LiveStreams.Dtos;
using game_x.application.Features.LiveStreams.Mapping;

namespace game_x.application.Features.LiveStreams.Queries.GetSchedulesByCriteria;

public sealed class GetSchedulesByCriteriaHandler(
    ICriteriaBuilder<LivestreamSchedule> builder,
    ILiveStreamRepo liveStreamRepo) : IQueryHandler<GetSchedulesByCriteriaQuery, PaginationResult<LiveStreamScheduleListItemDto>>
{
    public async Task<PaginationResult<LiveStreamScheduleListItemDto>> Handle(
        GetSchedulesByCriteriaQuery request,
        CancellationToken ct = default)
    {
        var items = await liveStreamRepo.GetsByCriteriaAsync(
            query => builder.Apply(
                query,
                filters: request.Filters,
                sorts: request.Sorts,
                keyword => ls => ls.Title.ToLowerInvariant().Contains(keyword.ToLowerInvariant())),
            request.PageIndex,
            request.PageSize,
            ct);
        var result = items.ToSearchResult();
        return result;
    }
}
