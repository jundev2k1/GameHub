using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Contract.Infrastructure.Caching;

public interface ILiveStreamManagerCacheService
{
    string[] GetAllStreamKeys();

    void ConnectLiveStream(LiveStreamStatusDto streamInfo);

    void DisconnnectLiveStream(string streamKey);

    void RemoveLiveStream(string streamKey);

    LiveStreamStatusDto? GetLiveStreamStatus(string streamKey);

    bool ContainsStreamKey(string streamKey);

    LiveStreamViewerDto GetViewerInfo(string streamKey, string clientId);

    void WatchLiveStream(LiveStreamViewerDto viewer);

    void UnwatchLiveStream(string streamKey, string clientId);

    string[] GetAllViewersByStreamKey(string streamKey);

    int GetViewerCount(string streamKey);
}
