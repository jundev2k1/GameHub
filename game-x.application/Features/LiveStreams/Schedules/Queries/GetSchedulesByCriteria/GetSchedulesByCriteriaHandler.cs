using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Extensions.FilterExtensions;
using game_x.application.Features.Games.Mapping;
using game_x.application.Features.LiveStreams.Schedules.Dtos;
using game_x.application.Features.LiveStreams.Schedules.Mapping;

namespace game_x.application.Features.LiveStreams.Schedules.Queries.GetSchedulesByCriteria;

public sealed class GetSchedulesByCriteriaHandler(
    ICriteriaBuilder<LivestreamSchedule> builder,
    ILiveStreamRepo liveStreamRepo,
    IFileManagerCacheService fileManagerCache) : IQueryHandler<GetSchedulesByCriteriaQuery, PaginationResult<LiveStreamScheduleListItemDto>>
{
    public async Task<PaginationResult<LiveStreamScheduleListItemDto>> Handle(
        GetSchedulesByCriteriaQuery request,
        CancellationToken ct = default)
    {
        var searchResult = await liveStreamRepo.GetsByCriteriaAsync(
            query => builder.Apply(
                query,
                filters: request.Filters,
                sorts: request.Sorts,
                keyword => ls => ls.Title.ToLower().Contains(keyword.ToLower()),
                LiveStreamFilterExtensions.Options),
            request.PageIndex,
            request.PageSize,
            ct);
        var avatarTasks = searchResult.Items
            .Select(async item => await GetImageUrlAsync(item, ct))
            .ToArray();
        var avatars = await Task.WhenAll(avatarTasks);
        var result = searchResult.ToSearchResult(avatars);
        return result;
    }

    private async Task<(Guid Id, string? Thumbnail, string? AssignedAvatar)> GetImageUrlAsync(LivestreamSchedule schedule, CancellationToken ct)
    {
        // Get thumbnail
        string? thumbnail = null;
        if (schedule.Thumbnail != null)
            thumbnail = await fileManagerCache.GetFileUrl(schedule.Thumbnail, ct);

        // Get assigned talent avatar
        string? avatar = null;
        if (schedule.AssignedTo != null && schedule.AssignedTo.Avatar != null)
            avatar = await fileManagerCache.GetFileUrl(schedule.AssignedTo.Avatar, ct);

        return (schedule.PublicId, thumbnail, avatar);
    }
}
