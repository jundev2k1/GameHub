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

    #region Stream Management
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

        var cacheListKey = $"{LiveStreamPrefix}streams";
        var streamList = GetAllStreamKeys();
        if (!streamList.Contains(streamInfo.StreamKey))
            streamList = [.. streamList, streamInfo.StreamKey];
        Set(cacheListKey, streamList);
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

        var viewerListCacheKey = $"{LiveStreamPrefix}streams:{streamKey}:viewers";
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
    #endregion

    #region Viewer Management
    public LiveStreamViewerDto? GetViewerInfo(string streamKey, string token)
    {
        var viewerCacheKey = $"{LiveStreamPrefix}streams:{streamKey}:viewers:{token}";
        return Get<LiveStreamViewerDto?>(viewerCacheKey);
    }

    public void InitViewerLiveStream(LiveStreamViewerDto viewer)
    {
        viewer.IsWatching = false;
        viewer.JoinAt = null;
        viewer.OutAt = null;

        var viewerCacheKey = $"{LiveStreamPrefix}streams:{viewer.StreamKey}:viewers:{viewer.Token}";
        Set(viewerCacheKey, viewer);
    }

    public void WatchLiveStream(LiveStreamViewerDto viewer)
    {
        // Mark viewer as watching
        viewer.IsWatching = true;
        if (!viewer.JoinAt.HasValue)
            viewer.JoinAt = DateTime.UtcNow;

        // Store viewer info
        var viewerCacheKey = $"{LiveStreamPrefix}streams:{viewer.StreamKey}:viewers:{viewer.Token}";
        Set(viewerCacheKey, viewer);

        // Update viewer list for the stream
        var updatedViewers = GetAllViewersByStreamKey(viewer.StreamKey) ?? [];
        var isExist = updatedViewers.ContainsKey(viewer.ViewerId)
            && updatedViewers[viewer.ViewerId].Contains(viewer.Token);
        if (isExist) return;

        var targetViewersArray = updatedViewers.FirstOrDefault(kvp => kvp.Key == viewer.ViewerId).Value ?? [];
        updatedViewers[viewer.ViewerId] = [.. targetViewersArray, viewer.Token];
        var viewerListCacheKey = $"{LiveStreamPrefix}streams:{viewer.StreamKey}:viewers";
        Set(viewerListCacheKey, updatedViewers);
    }

    public void UnwatchLiveStream(LiveStreamViewerDto viewer)
    {
        // Retrieve viewer info
        var viewersKey = $"{LiveStreamPrefix}streams:{viewer.StreamKey}:viewers:{viewer.Token}";

        // Mark viewer as not watching
        viewer.IsWatching = false;
        viewer.OutAt = DateTime.UtcNow;
        Set(viewersKey, viewer);

        // Update viewer list for the stream
        var updatedViewers = GetAllViewersByStreamKey(viewer.StreamKey) ?? [];

        var targetViewersArray = updatedViewers.FirstOrDefault(kvp => kvp.Key == viewer.ViewerId).Value ?? [];
        updatedViewers[viewer.ViewerId] = [.. targetViewersArray.Where(t => t != viewer.Token)];
        var viewerListCacheKey = $"{LiveStreamPrefix}streams:{viewer.StreamKey}:viewers";
        Set(viewerListCacheKey, updatedViewers);
    }

    public Dictionary<string, string[]> GetAllViewersByStreamKey(string streamKey)
    {
        var viewerListCacheKey = $"{LiveStreamPrefix}streams:{streamKey}:viewers";
        return Get<Dictionary<string, string[]>>(viewerListCacheKey) ?? [];
    }

    public LiveStreamViewerDto[] GetAllViewerInfosByStreamKey(string streamKey)
    {
        var allKeys = GetAllViewersByStreamKey(streamKey);
        var viewers = allKeys
            .SelectMany(kvp => kvp.Value)
            .Select(token => Get<LiveStreamViewerDto>(
                $"{LiveStreamPrefix}streams:{streamKey}:viewers:{token}"))
            .Where(dto => dto != null)
            .ToArray();
        return viewers!;
    }

    public void RemoveViewersByStreamKey(string streamKey)
    {
        var viewerListCacheKey = $"{LiveStreamPrefix}streams:{streamKey}:viewers";
        var allViewersByStreamKey = GetAllViewersByStreamKey(streamKey);
        foreach (var viewerId in allViewersByStreamKey)
        {
            var viewerCacheKey = $"{viewerListCacheKey}:{viewerId}";
            Remove(viewerCacheKey);
        }
        Remove(viewerListCacheKey);
    }

    public int GetViewerCount(string streamKey)
    {
        var streamViewers = GetAllViewerInfosByStreamKey(streamKey);
        return streamViewers.GroupBy(v => v.ViewerId).Count(gr => gr.Any(v => v.IsWatching));
    }
    #endregion

    #region Chat Management
    public void InitMessagesForStream(string streamKey)
    {
        var cacheKey = $"{LiveStreamPrefix}streams:{streamKey}:messages";
        Set(cacheKey, new Dictionary<Guid, DateTime>());
    }

    public Dictionary<Guid, DateTime> GetAllMessageKey(string streamKey)
    {
        var cacheKey = $"{LiveStreamPrefix}streams:{streamKey}:messages";
        var result = Get<Dictionary<Guid, DateTime>>(cacheKey) ?? [];
        return result;
    }

    public LiveStreamChatMessageDto[] GetAdjacentMessages(string streamKey, Guid messageId, bool isNext, int count = 20)
    {
        var allMessageKeys = GetAllMessageKey(streamKey)
            .Select((kvp, index) => (Index: index, MessageId: kvp.Key, SentAt: kvp.Value))
            .OrderByDescending(i => i.SentAt)
            .ToArray();
        if (allMessageKeys.Length == 0) return [];

        // Find the target message
        var targetItem = allMessageKeys.FirstOrDefault(kvp => kvp.MessageId == messageId);
        if (targetItem.MessageId == Guid.Empty)
            throw new NotFoundException(nameof(messageId), messageId);

        // Calculate skip count based on direction
        // If isNext is true, skip 1 to move to the next item
        // If isNext is false, skip -count to move to the previous items
        var skipCount = isNext ? 1 : -count;

        // Take adjacent messages
        var result = allMessageKeys
            .Skip(skipCount)
            .Take(count)
            .Select(kvp => GetMessageDetail(streamKey, kvp.MessageId))
            .Where(dto => dto != null)
            .ToArray();
        return result!;
    }

    public LiveStreamChatMessageDto? GetMessageDetail(string streamKey, Guid messageId)
    {
        var messageCacheKey = $"{LiveStreamPrefix}streams:{streamKey}:messages:{messageId}";
        return Get<LiveStreamChatMessageDto?>(messageCacheKey);
    }

    public void AddMessageToStream(string streamKey, LiveStreamChatMessageDto message)
    {
        // Store message detail
        var cacheKey = $"{LiveStreamPrefix}{streamKey}:messages:{message.Id}";
        Set(cacheKey, message);

        // Update message keys list
        var allMessageKeys = GetAllMessageKey(streamKey);
        allMessageKeys[message.Id] = message.SentAt;
        var allMessageKeysCacheKey = $"{LiveStreamPrefix}{streamKey}:messages";
        Set(allMessageKeysCacheKey, allMessageKeys);
    }

    public void RemoveMessageFromStream(string streamKey, Guid messageId)
    {
        // Remove message detail
        var cacheKey = $"{LiveStreamPrefix}{streamKey}:messages:{messageId}";
        Remove(cacheKey);

        // Update message keys list
        var allMessageKeys = GetAllMessageKey(streamKey);
        allMessageKeys.Remove(messageId);
        var allMessageKeysCacheKey = $"{LiveStreamPrefix}{streamKey}:messages";
        Set(allMessageKeysCacheKey, allMessageKeys);
    }
    #endregion
}
