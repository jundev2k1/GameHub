using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Events.OnLiveStreamJoined;

public sealed class OnLiveStreamJoinedHandler(
    ILiveStreamManagerCacheService liveStreamManager,
    IUnitOfWork unitOfWork,
    ILiveStreamChatRepo liveStreamChatRepo,
    ILiveStreamHubService liveStreamHub) : IApplicationEventHandler<OnLiveStreamJoinedEvent>
{
    public async Task Handle(OnLiveStreamJoinedEvent @event, CancellationToken ct = default)
    {
        var streamKey = @event.StreamKey;
        var viewer = @event.Viewer;

        await CreateStreamMessage(streamKey, viewer, ct);
        await NotifyUserJoinedForHost(streamKey, viewer);
    }

    private async Task CreateStreamMessage(string streamKey, LiveStreamViewerDto viewer, CancellationToken ct)
    {
        var chatMessage = LiveStreamChatMessage.Create(
            viewer.ViewerId,
            string.Empty,
            LiveStreamChatMessageType.UserJoined);
        await liveStreamChatRepo.CreateAsync(chatMessage, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // Add message to cache in stream
        liveStreamManager.AddMessageToStream(streamKey, chatMessage.Adapt<LiveStreamChatMessageDto>());
    }

    private async Task NotifyUserJoinedForHost(string streamKey, LiveStreamViewerDto viewer)
    {
        var userDevices = liveStreamManager.GetViewerDevicesByViewerId(streamKey, viewer.ViewerId)
            .Select(token => liveStreamManager.GetViewerInfo(streamKey, token))
            .Where(ud => ud is not null)
            .ToArray();
        if (userDevices.Length == 0) return;

        var firstDevice = userDevices[0];
        var viewerInfo = new LiveStreamViewerInfoDto
        {
            ViewerId = viewer.ViewerId,
            ViewerName = viewer.ViewerName,
            ViewerAvatar = viewer.ViewerAvatar,
            DeviceInfos = [.. userDevices.Select(ud => new ViewerDeviceInfoDto
            {
                DeviceName = ud!.DeviceInfo,
                IsWatching = ud!.IsWatching,
                JoinAi = ud!.JoinAt,
                OutAt = ud!.OutAt
            })],
        };
        await liveStreamHub.NotifyUserJoined(streamKey, viewerInfo);
    }
}
