using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Contract.Infrastructure.Caching;

public interface ILiveStreamManagerCacheService
{
    string[] GetAllStreamKeys();

    void InitLiveStream(LiveStreamStatusDto streamInfo);

    void ConnectLiveStream(string streamKey);

    void DisconnnectLiveStream(string streamKey);

    void RemoveLiveStream(string streamKey);

    bool IsExistLiveStream(string streamKey);

    LiveStreamStatusDto? GetLiveStreamStatus(string streamKey);

    LiveStreamViewerDto? GetViewerInfo(string streamKey, string token);

    void InitViewerLiveStream(LiveStreamViewerDto viewer);

    void WatchLiveStream(LiveStreamViewerDto viewer);

    void UnwatchLiveStream(LiveStreamViewerDto viewer);

    Dictionary<string, string[]> GetAllViewersByStreamKey(string streamKey);

    int GetViewerCount(string streamKey);
}
