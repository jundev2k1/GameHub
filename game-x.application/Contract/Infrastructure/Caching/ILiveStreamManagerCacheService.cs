using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Gifts.Dtos;
using game_x.application.Features.LiveStreams.Streaming.Dtos;

namespace game_x.application.Contract.Infrastructure.Caching;

public interface ILiveStreamManagerCacheService
{
    #region Stream Management
    Dictionary<string, string[]> GetAllStreamKeys();

    void InitLiveStream(LiveStreamStatusDto streamInfo);

    void ConnectLiveStream(string streamKey);

    void DisconnnectLiveStream(string streamKey);

    void AddBlackList(string streamKey, BlackListItemDto statusDto);

    void RemoveBlackList(string streamKey, string userId, BlackListAction action);

    void RemoveLiveStream(string streamKey);

    bool IsExistLiveStream(string streamKey);

    LiveStreamStatusDto? GetLiveStreamStatus(string streamKey);
    #endregion

    #region Viewer Management
    LiveStreamViewerDto? GetViewerInfo(string streamKey, string token);

    void InitViewerLiveStream(LiveStreamViewerDto viewer);

    void WatchLiveStream(LiveStreamViewerDto viewer);

    void UnwatchLiveStream(LiveStreamViewerDto viewer);

    Dictionary<string, string[]> GetAllViewersByStreamKey(string streamKey);

    string[] GetViewerDevicesByViewerId(string streamKey, string viewerId);

    void RemoveViewersByStreamKey(string streamKey);
    #endregion

    #region View management
    string[] GetViewerChangeList();

    void CleanViewerChangeList();

    void MarkAsStreamViewChange(string streamKey);

    int GetViewerCount(string streamKey);
    #endregion

    #region Chat Message Management
    void InitMessagesForStream(string streamKey);

    Dictionary<Guid, DateTime> GetAllMessageKey(string streamKey);

    LiveStreamChatMessageDto[] GetAdjacentMessages(
        string streamKey,
        Guid? messageId,
        bool isNext,
        int count = 20);

    LiveStreamChatMessageDto? GetMessageDetail(string streamKey, Guid messageId);

    void AddMessageToStream(string streamKey, LiveStreamChatMessageDto message);

    void RemoveAllMessageByStreamKey(string streamKey);

    void RemoveMessageFromStream(string streamKey, Guid messageId);
    #endregion

    #region Gift Management
    Task<LiveStreamGiftClientDto[]> GetAllActiveGiftsAsync(CancellationToken ct = default);

    Task RefreshGiftCacheAsync(CancellationToken ct = default);
    #endregion

    #region Donation Management
    Dictionary<Guid, DateTime> GetStreamDonationKeys(string streamKey);

    Task SetInitDonations(string streamKey, CancellationToken ct = default);

    void AddDonationToStream(string streamKey, LiveStreamDonationDto donation);

    LiveStreamDonationDto? GetDonationDetail(string streamKey, Guid donationId);

    void RemoveDonationsByStreamKey(string streamKey);

    void RemoveDonationFromStream(string streamKey, Guid donationId);
    #endregion
}
