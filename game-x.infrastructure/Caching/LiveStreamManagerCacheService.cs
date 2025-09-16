using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Exceptions;
using game_x.application.Features.LiveStreams.Dtos;
using game_x.share.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching;

public sealed class LiveStreamManagerCacheService(IMemoryCache cache)
    : CacheService(cache), ILiveStreamManagerCacheService
{
    private const string LiveStreamPrefix = "livestream:";
    private const string LiveStreamViewersPrefix = "livestream:viewers:";

    public string[] GetAllStreamKeys()
    {
        var cacheKey = $"{LiveStreamPrefix}streams";
        var result = Get<string[]>(cacheKey) ?? [];
        return result;
    }

    public void InitLiveStream(LiveStreamStatusDto streamInfo)
    {
        if (streamInfo.StreamKey.IsNullOrWhiteSpace())
            throw new ArgumentException("Stream key cannot be null or empty.", streamInfo.StreamKey);

        var cacheKey = $"{LiveStreamPrefix}streams:{streamInfo.StreamKey}";
        Set(cacheKey, streamInfo);
    }

    public void ConnectLiveStream(string streamKey)
    {
        var cacheKey = $"{LiveStreamPrefix}streams:{streamKey}";
        var streamInfo = Get<LiveStreamStatusDto?>(cacheKey)
            ?? throw new NotFoundException(nameof(streamKey), streamKey);

        if (streamInfo.StreamKey.IsNullOrWhiteSpace())
            throw new ArgumentException("Stream key cannot be null or empty.", streamInfo.StreamKey);

        streamInfo.IsLive = true;
        Set(cacheKey, streamInfo);
    }

    public void DisconnnectLiveStream(string streamKey)
    {
        var cacheKey = $"{LiveStreamPrefix}streams:{streamKey}";
        var targetStream = Get<LiveStreamStatusDto>(cacheKey)
            ?? throw new NotFoundException(nameof(streamKey), streamKey);

        targetStream.IsLive = false;
        targetStream.OfflineAt = DateTime.UtcNow;
        Set(cacheKey, targetStream);
    }

    public void AddBlackList(string streamKey, BlackListItemDto statusDto)
    {
        var targetStream = GetLiveStreamStatus(streamKey)
            ?? throw new NotFoundException(nameof(streamKey), streamKey);
        if (targetStream.BlackList.Any(bl => bl.UserId == statusDto.UserId && bl.Action == statusDto.Action))
            throw new BadRequestException("Black list item exists.");

        targetStream.BlackList = [.. targetStream.BlackList, statusDto];

        var cacheKey = $"{LiveStreamPrefix}streams:{targetStream.StreamKey}";
        Set(cacheKey, targetStream);
    }

    public void RemoveBlackList(string streamKey, string userId, BlackListAction action)
    {
        var targetStream = GetLiveStreamStatus(streamKey)
            ?? throw new NotFoundException(nameof(streamKey), streamKey);
        targetStream.BlackList = [.. targetStream.BlackList.Where(bl => bl.UserId == userId && bl.Action == action)];

        var cacheKey = $"{LiveStreamPrefix}streams:{targetStream.StreamKey}";
        Set(cacheKey, targetStream);
    }

    public void RemoveLiveStream(string streamKey)
    {
        var streamList = GetAllStreamKeys()
            .Where(key => key != streamKey)
            .ToArray();
        var streamListCacheKey = $"{LiveStreamPrefix}streams";
        Set(streamListCacheKey, streamList);

        var streamDetailCacheKey = $"{LiveStreamPrefix}streams:{streamKey}";
        Remove(streamDetailCacheKey);

        var viewerListCacheKey = $"{LiveStreamViewersPrefix}{streamKey}";
        var allViewersByStreamKey = GetAllViewersByStreamKey(streamKey);
        foreach (var viewerId in allViewersByStreamKey)
        {
            var viewerCacheKey = $"{viewerListCacheKey}:{viewerId}";
            Remove(viewerCacheKey);
        }
        Remove(viewerListCacheKey);
    }

    public bool IsExistLiveStream(string streamKey)
    {
        var cacheKey = $"{LiveStreamPrefix}streams:{streamKey}";
        return Get<LiveStreamStatusDto?>(cacheKey) != null;
    }

    public LiveStreamStatusDto? GetLiveStreamStatus(string streamKey)
    {
        var cacheKey = $"{LiveStreamPrefix}streams:{streamKey}";
        return Get<LiveStreamStatusDto?>(cacheKey);
    }

    public LiveStreamViewerDto? GetViewerInfo(string streamKey, string token)
    {
        var viewerCacheKey = $"{LiveStreamViewersPrefix}{streamKey}:{token}";
        return Get<LiveStreamViewerDto?>(viewerCacheKey);
    }

    public void InitViewerLiveStream(LiveStreamViewerDto viewer)
    {
        viewer.IsWatching = false;
        viewer.JoinAt = null;
        viewer.OutAt = null;

        var viewerCacheKey = $"{LiveStreamViewersPrefix}{viewer.StreamKey}:{viewer.Token}";
        Set(viewerCacheKey, viewer);
    }

    public void WatchLiveStream(LiveStreamViewerDto viewer)
    {
        // Mark viewer as watching
        viewer.IsWatching = true;
        if (!viewer.JoinAt.HasValue)
            viewer.JoinAt = DateTime.UtcNow;

        // Store viewer info
        var viewerCacheKey = $"{LiveStreamViewersPrefix}{viewer.StreamKey}:{viewer.Token}";
        Set(viewerCacheKey, viewer);

        // Update viewer list for the stream
        var updatedViewers = GetAllViewersByStreamKey(viewer.StreamKey) ?? [];
        var isExist = updatedViewers.ContainsKey(viewer.ViewerId)
            && updatedViewers[viewer.ViewerId].Contains(viewer.Token);
        if (isExist) return;

        var targetViewersArray = updatedViewers.FirstOrDefault(kvp => kvp.Key == viewer.ViewerId).Value ?? [];
        updatedViewers[viewer.ViewerId] = [.. targetViewersArray, viewer.Token];
        var viewerListCacheKey = $"{LiveStreamViewersPrefix}{viewer.StreamKey}";
        Set(viewerListCacheKey, updatedViewers);
    }

    public void UnwatchLiveStream(LiveStreamViewerDto viewer)
    {
        // Retrieve viewer info
        var viewersKey = $"{LiveStreamViewersPrefix}{viewer.StreamKey}:{viewer.Token}";

        // Mark viewer as not watching
        viewer.IsWatching = false;
        viewer.OutAt = DateTime.UtcNow;
        Set(viewersKey, viewer);

        // Update viewer list for the stream
        var updatedViewers = GetAllViewersByStreamKey(viewer.StreamKey) ?? [];

        var targetViewersArray = updatedViewers.FirstOrDefault(kvp => kvp.Key == viewer.ViewerId).Value ?? [];
        updatedViewers[viewer.ViewerId] = [.. targetViewersArray.Where(t => t != viewer.Token)];
        var viewerListCacheKey = $"{LiveStreamViewersPrefix}{viewer.StreamKey}";
        Set(viewerListCacheKey, updatedViewers);
    }

    public Dictionary<string, string[]> GetAllViewersByStreamKey(string streamKey)
    {
        var viewerListCacheKey = $"{LiveStreamViewersPrefix}{streamKey}";
        return Get<Dictionary<string, string[]>>(viewerListCacheKey) ?? [];
    }

    public LiveStreamViewerDto[] GetAllViewerInfosByStreamKey(string streamKey)
    {
        var allKeys = GetAllViewersByStreamKey(streamKey);
        var viewers = allKeys
            .Select(key => Get<LiveStreamViewerDto>(
                $"{LiveStreamViewersPrefix}{streamKey}:{key}"))
            .Where(dto => dto != null)
            .ToArray();
        return viewers!;
    }

    public int GetViewerCount(string streamKey)
    {
        var streamViewers = GetAllViewerInfosByStreamKey(streamKey);
        return streamViewers.Count(v => v.IsWatching);
    }
}
