using game_x.application.Contract.Infrastructure.SignalR.Dtos.LiveStream;
using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Contract.Infrastructure.SignalR.Services;

public interface ILiveStreamHubService
{
    Task NotifyStreamReconnected(string streamKey);

    Task NotifyStreamDisconnected(string streamKey);

    Task NotifyUserJoined(string streamKey, LiveStreamViewerInfoDto viewer);

    Task NotifyUserLeft(string streamKey, string viewerId);

    Task NotifyCancelStream(string streamKey, string reason);

    Task PerformActionMember(string streamKey, string viewerId, LiveStreamBanInfo banInfo);

    Task RefreshViewCount(string streamKey, int viewCount);

    Task SendChatMessage(string streamKey, LiveStreamChatMessageDto message);
}
