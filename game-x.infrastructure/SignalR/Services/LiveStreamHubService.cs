using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.LiveStream;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Features.LiveStreams.Streaming.Dtos;
using game_x.infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace game_x.infrastructure.SignalR.Services;

public sealed class LiveStreamHubService(IHubContext<LiveStreamHub, ILiveStreamHub> hubContext)
    : ILiveStreamHubService, IHubServices
{
    public async Task NotifyStreamReconnected(string streamKey)
    {
        // Notify all viewers in the stream
        await hubContext.Clients
            .Group($"stream-{streamKey}")
            .OnStreamReconnected();
    }

    public async Task NotifyStreamDisconnected(string streamKey)
    {
        // Notify all viewers in the stream
        await hubContext.Clients
            .Group($"stream-{streamKey}")
            .OnStreamDisconnected();
    }

    public async Task NotifyUserJoined(string streamKey, LiveStreamViewerInfoDto viewer)
    {
        // Notify all host in the stream
        await hubContext.Clients
            .Group($"stream-{streamKey}-host")
            .OnUserJoined(viewer);
    }

    public async Task NotifyUserLeft(string streamKey, string viewerId)
    {
        // Notify all host in the stream
        await hubContext.Clients
            .Group($"stream-{streamKey}-host")
            .OnUserLeft(viewerId);
    }

    public async Task NotifyCancelStream(string streamKey, string reason)
    {
        // Notify all viewers in the stream
        await hubContext.Clients
            .Group($"stream-{streamKey}")
            .OnStreamCanceled(reason);
    }

    public async Task NotifyEndStream(string streamKey)
    {
        await hubContext.Clients
            .Group($"stream-{streamKey}")
            .OnStreamEnded();
    }

    public async Task PerformActionMember(string streamKey, string viewerId, LiveStreamBanInfo banInfo)
    {
        // Notify for the viewer who is banned/muted/kicked
        await hubContext.Clients
            .Group($"stream-{streamKey}-member-{viewerId}")
            .OnMemberAction(banInfo);
    }

    public async Task RefreshViewCount(string streamKey, int viewCount)
    {
        // Notify all viewers in the stream
        await hubContext.Clients
            .Group($"stream-{streamKey}")
            .OnViewChange(viewCount);
    }

    public async Task SendChatMessage(string streamKey, LiveStreamChatMessageDto message)
    {
        // Notify all viewers in the stream
        await hubContext.Clients
            .Group($"stream-{streamKey}")
            .OnReceiveMessage(message);
    }

    public async Task NotifyMessageFailed(string streamKey, string userId, string messageId)
    {
        // Notify for the viewer who sent the message
        await hubContext.Clients
            .Group($"stream-{streamKey}-member-{userId}")
            .NotifyMessageFailed(messageId);
    }

    public async Task NotifyMessageDeleted(string streamKey, Guid messageId)
    {
        await hubContext.Clients
            .Group($"stream-{streamKey}")
            .OnMessageDeleted(messageId);
    }
}
