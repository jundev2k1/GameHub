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
        Task<(Guid Id, string? Thumbnail, string? AssignedAvatar)>[] avatarTasks = [.. searchResult.Items.Select(async item =>
        {
            // Get thumbnail
            string? thumbnail = null;
            if (item.Thumbnail != null)
            {
                var thumbnailInfo = await fileManagerCache.GetFileInfo(item.Thumbnail);
                thumbnail = thumbnailInfo?.Url;
            }

            // Get assigned talent avatar
            string? avatar = null;
            if (item.AssignedTo != null && item.AssignedTo.Avatar != null)
            {
                var avatarInfo = await fileManagerCache.GetFileInfo(item.AssignedTo.Avatar);
                avatar = avatarInfo?.Url;
            }

            return (item.PublicId, thumbnail, avatar);
        })];
        var avatars = await Task.WhenAll(avatarTasks);
        var result = searchResult.ToSearchResult(avatars);
        return result;
    }
}
