using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Features.LiveStreams.Schedules.Dtos;

namespace game_x.application.Features.LiveStreams.Streaming.Queries.GetActiveStreams;

public sealed class GetActiveStreamsHandler(
    ILiveStreamManagerCacheService liveStreamManager,
    IFileManagerCacheService fileManagerCache) : IQueryHandler<GetActiveStreamsQuery, PaginationResult<LiveStreamScheduleClientItemDto>>
{
    public async Task<PaginationResult<LiveStreamScheduleClientItemDto>> Handle(GetActiveStreamsQuery request, CancellationToken ct = default)
    {
        // Get all active streams
        var streamTasks = liveStreamManager.GetAllStreamKeys()
            .Select(GetSearchItem);
        var allActiveStreams = await Task.WhenAll(streamTasks);
        var streamList = allActiveStreams.Where(stream => stream is not null);

        // Paginate
        var totalCount = streamList.Count();
        var totalPageCount = (int)Math.Ceiling((decimal)totalCount / request.PageSize);
        var items = streamList
            .OrderByDescending(stream => stream!.ViewCount)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToArray();

        // Return result
        var result = new PaginationResult<LiveStreamScheduleClientItemDto>(
            items!,
            totalCount,
            totalPageCount,
            request.PageIndex,
            request.PageSize);
        return result;
    }

    private async Task<LiveStreamScheduleClientItemDto?> GetSearchItem(string streamKey)
    {
        // Get stream status
        var stream = liveStreamManager.GetLiveStreamStatus(streamKey);

        // Skip offline streams or streams that have been offline for more than 5 minutes
        if (stream is null
            || !stream.IsLive
                && stream.OfflineAt.HasValue
                && (DateTime.UtcNow - stream.OfflineAt.Value).Minutes > 5)
            return null;

        // Get thumbnail
        if (stream != null && stream.ThumbnailId.HasValue)
        {
            var thumbnail = await fileManagerCache.GetFileUrl(stream.ThumbnailId.Value);
            stream.Thumbnail = thumbnail?.Url;
        }

        // Map to DTO
        var dto = stream.Adapt<LiveStreamScheduleClientItemDto>();

        // Get viewer count
        var viewCount = liveStreamManager.GetViewerCount(streamKey);
        dto.ViewCount = viewCount;

        return dto;
    }
}
