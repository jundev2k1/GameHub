using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.SignalR.Services;

namespace game_x.application.Events.OnLiveStreamLeft;

public sealed class OnLiveStreamLeftHandler(
    ILiveStreamManagerCacheService liveStreamManager,
    ILiveStreamHubService liveStreamHub) : IApplicationEventHandler<OnLiveStreamLeftEvent>
{
    public async Task Handle(OnLiveStreamLeftEvent @event, CancellationToken ct = default)
    {
        var streamKey = @event.StreamKey;
        var viewer = @event.Viewer;

        // Mark as stream view change to update viewer count
        liveStreamManager.MarkAsStreamViewChange(streamKey);

        await liveStreamHub.NotifyUserLeft(streamKey, viewer.ViewerId);
    }
}
