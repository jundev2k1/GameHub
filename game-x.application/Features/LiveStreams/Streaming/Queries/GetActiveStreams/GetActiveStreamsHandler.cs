using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Schedules.Dtos;

namespace game_x.application.Features.LiveStreams.Streaming.Queries.GetActiveStreams;

public sealed class GetActiveStreamsHandler(
    ILiveStreamManagerCacheService liveStreamManager,
    IFileManagerCacheService fileManagerCache,
    ILiveStreamReminderRepo streamReminderRepo) : IQueryHandler<GetActiveStreamsQuery, PaginationResult<LiveStreamScheduleClientItemDto>>
{
    public async Task<PaginationResult<LiveStreamScheduleClientItemDto>> Handle(GetActiveStreamsQuery request, CancellationToken ct = default)
    {
        // Get all active streams
        var streamTasks = liveStreamManager.GetAllStreamKeys()
            .SelectMany(kvp => kvp.Value)
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
        var dataResult = await MapDetailDataAsync(items!, ct);
        var result = new PaginationResult<LiveStreamScheduleClientItemDto>(
            dataResult,
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
        if (stream is null) return null;

        // Get thumbnail
        if (stream != null && stream.ThumbnailId.HasValue)
            stream.Thumbnail = await fileManagerCache.GetFileUrl(stream.ThumbnailId.Value);

        //Get talent avatar
        if (stream != null && stream.AssignedTo?.AvatarId != null)
            stream.AssignedTo.Avatar = await fileManagerCache.GetFileUrl(stream.AssignedTo.AvatarId.Value);

        // Map to DTO
        var dto = stream.Adapt<LiveStreamScheduleClientItemDto>();
        return dto;
    }

    private async Task<IEnumerable<LiveStreamScheduleClientItemDto>> MapDetailDataAsync(
        IEnumerable<LiveStreamScheduleClientItemDto> items,
        CancellationToken ct)
    {
        var streamIds = items.Select(x => x.Id);
        var reminders = await streamReminderRepo.GetStreamRemindersAsync(streamIds, ct);

        foreach (var item in items)
        {
            if (reminders.TryGetValue(item.Id, out var channels))
                item.ReminderChannels = channels;

            // Get viewer count
            var viewCount = liveStreamManager.GetViewerCount(item.StreamKey);
            item.ViewCount = viewCount;
        }

        return items;
    }
}
