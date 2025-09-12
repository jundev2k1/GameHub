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

    public void ConnectLiveStream(LiveStreamStatusDto streamInfo)
    {
        if (streamInfo.StreamKey.IsNullOrWhiteSpace())
            throw new ArgumentException("Stream key cannot be null or empty.", streamInfo.StreamKey);

        streamInfo.IsLive = true;

        var cacheKey = $"{LiveStreamPrefix}streams:{streamInfo.StreamKey}";
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

    public LiveStreamStatusDto? GetLiveStreamStatus(string streamKey)
    {
        return Get<LiveStreamStatusDto>(streamKey);
    }

    public bool ContainsStreamKey(string streamKey)
    {
        return GetAllStreamKeys().Contains(streamKey);
    }

    public void WatchLiveStream(LiveStreamViewerDto viewer)
    {
        // Mark viewer as watching
        viewer.IsWatching = true;

        // Store viewer info
        var viewerCacheKey = $"{LiveStreamViewersPrefix}{viewer.StreamKey}:{viewer.ClientId}:{viewer.DeviceInfo}";
        Set(viewerCacheKey, viewer);

        // Update viewer list for the stream
        var viewerListCacheKey = $"{LiveStreamViewersPrefix}{viewer.StreamKey}";
        var allViewersByStreamKey = Get<string[]?>(viewerListCacheKey) ?? [];
        if (!allViewersByStreamKey.Contains(viewer.ClientId))
        {
            string[] updatedViewers = [.. allViewersByStreamKey, viewer.ClientId];
            Set(viewerListCacheKey, updatedViewers);
        }
    }

    public void UnwatchLiveStream(string streamKey, string clientId, string devideInfo)
    {
        var viewersKey = $"{LiveStreamViewersPrefix}{streamKey}:{clientId}:{devideInfo}";
        var viewer = Get<LiveStreamViewerDto>(viewersKey)
            ?? throw new NotFoundException(nameof(clientId), clientId);

        viewer.IsWatching = false;
        viewer.OutAt = DateTime.UtcNow;
        Set(viewersKey, viewer);
    }

    public string[] GetAllViewersByStreamKey(string streamKey)
    {
        var viewerListCacheKey = $"{LiveStreamViewersPrefix}{streamKey}";
        return Get<string[]>(viewerListCacheKey) ?? [];
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
