using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Features.LiveStreams.Queries.GetActiveStreams;

public sealed class GetActiveStreamsHandler(
    ILiveStreamManagerCacheService liveStreamManager) : IQueryHandler<GetActiveStreamsQuery, PaginationResult<LiveStreamScheduleClientItemDto>>
{
    public async Task<PaginationResult<LiveStreamScheduleClientItemDto>> Handle(GetActiveStreamsQuery request, CancellationToken ct = default)
    {
        var allStreams = liveStreamManager.GetAllStreamKeys()
            .Select(GetSearchItem)
            .OrderByDescending(stream => stream.ViewCount);
        var totalCount = allStreams.Count();
        var totalPageCount = (int)Math.Ceiling((decimal)totalCount / request.PageSize);
        var items = allStreams
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToArray();
        var result = new PaginationResult<LiveStreamScheduleClientItemDto>(
            items,
            totalCount,
            totalPageCount,
            request.PageIndex,
            request.PageSize);
        return await Task.FromResult(result);
    }

    private LiveStreamScheduleClientItemDto GetSearchItem(string streamKey)
    {
        var stream = liveStreamManager.GetLiveStreamStatus(streamKey);
        var viewCount = liveStreamManager.GetViewerCount(streamKey);

        var dto = stream.Adapt<LiveStreamScheduleClientItemDto>();
        dto.ViewCount = viewCount;
        return dto;
    }
}
