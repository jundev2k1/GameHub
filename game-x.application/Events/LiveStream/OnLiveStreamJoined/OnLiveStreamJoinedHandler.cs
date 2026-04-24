using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Features.LiveStreams.Streaming.Dtos;

namespace game_x.application.Events.LiveStream.OnLiveStreamJoined;

public sealed class OnLiveStreamJoinedHandler(
    ILiveStreamManagerCacheService liveStreamManager,
    ILiveStreamHubService liveStreamHub) : IApplicationEventHandler<OnLiveStreamJoinedEvent>
{
    public async Task Handle(OnLiveStreamJoinedEvent @event, CancellationToken ct = default)
    {
        var streamKey = @event.StreamKey;
        var viewer = @event.Viewer;

        // Mark as stream view change to update viewer count
        liveStreamManager.MarkAsStreamViewChange(streamKey);

        await NotifyUserJoinedForHost(streamKey, viewer);
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
                JoinAt = ud!.JoinAt,
                OutAt = ud!.OutAt
            })],
        };
        await liveStreamHub.NotifyUserJoined(streamKey, viewerInfo);
    }
}
