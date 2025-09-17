using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Games.Mapping;
using game_x.application.Features.LiveStreams.Dtos;
using game_x.application.Features.LiveStreams.Mapping;

namespace game_x.application.Features.LiveStreams.Queries.GetSchedulesByCriteria;

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
                keyword => ls => ls.Title.ToLowerInvariant().Contains(keyword.ToLowerInvariant())),
            request.PageIndex,
            request.PageSize,
            ct);
        Task<(Guid Id, string Url)>[] avatarTasks = [.. searchResult.Items.Select(async item =>
        {
            if (item.AssignedTo is null || item.AssignedTo.Avatar is null)
                return (item.PublicId, string.Empty);

            var avatarInfo = await fileManagerCache.GetImageUrl(item.AssignedTo.Avatar);
            if (avatarInfo is null) return (item.PublicId, string.Empty);

            return (item.PublicId, avatarInfo.Url);
        })];
        var avatars = await Task.WhenAll(avatarTasks);
        var result = searchResult.ToSearchResult(avatars);
        return result;
    }
}
