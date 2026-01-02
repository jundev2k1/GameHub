using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.LiveStream;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Features.LiveStreams.Streaming.Dtos;
using game_x.infrastructure.SignalR.Facade;
using game_x.infrastructure.SignalR.Hubs;

namespace game_x.infrastructure.SignalR.Services;

public sealed class LiveStreamHubService(LiveStreamHubFacade<LiveStreamHub, ILiveStreamHub> hub)
    : ILiveStreamHubService, IHubServices
{
    public async Task NotifyStreamReconnected(string streamKey)
    {
        // Notify all viewers in the stream
        await hub.Stream(streamKey).OnStreamReconnected();
    }

    public async Task NotifyStreamDisconnected(string streamKey)
    {
        // Notify all viewers in the stream
        await hub.Stream(streamKey).OnStreamDisconnected();
    }

    public async Task NotifyUserJoined(string streamKey, LiveStreamViewerInfoDto viewer)
    {
        // Notify all hosts in the stream
        await hub.StreamHost(streamKey).OnUserJoined(viewer);
    }

    public async Task NotifyUserLeft(string streamKey, string viewerId)
    {
        // Notify all hosts in the stream
        await hub.StreamHost(streamKey).OnUserLeft(viewerId);
    }

    public async Task NotifyCancelStream(string streamKey, string reason)
    {
        // Notify all viewers in the stream
        await hub.Stream(streamKey).OnStreamCancelled(reason);
    }

    public async Task NotifyEndStream(string streamKey)
    {
        await hub.Stream(streamKey).OnStreamEnded();
    }

    public async Task PerformActionMember(string streamKey, string viewerId, LiveStreamBanInfo banInfo)
    {
        // Notify for the viewer who is banned/muted/kicked
        await hub.StreamMember(streamKey, viewerId).OnMemberAction(banInfo);
    }

    public async Task RefreshViewCount(string streamKey, int viewCount)
    {
        // Notify all viewers in the stream
        await hub.Stream(streamKey).OnViewChange(viewCount);
    }

    public async Task SendChatMessage(string streamKey, LiveStreamChatMessageDto message)
    {
        // Notify all viewers in the stream
        await hub.Stream(streamKey).OnReceiveMessage(message);
    }

    public async Task NotifyMessageFailed(string streamKey, string userId, string messageId)
    {
        // Notify for the viewer who sent the message
        await hub.StreamMember(streamKey, userId).NotifyMessageFailed(messageId);
    }

    public async Task NotifyMessageDeleted(string streamKey, Guid messageId)
    {
        await hub.Stream(streamKey).OnMessageDeleted(messageId);
    }

    public async Task NotifyDonationCompleted(string streamKey, LiveStreamDonationDto donation)
    {
        await hub.Stream(streamKey).OnDonationCompleted(donation);
    }
}