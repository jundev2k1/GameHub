using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.SignalR.Services;

namespace game_x.application.Features.LiveStreams.Commands.UnpublishStream;

public sealed class UnpublishStreamHandler(
    ILiveStreamManagerCacheService liveStreamManager,
    ILiveStreamHubService liveStreamHub) : ICommandHandler<UnpublishStreamCommand>
{
    public async Task<Unit> Handle(UnpublishStreamCommand request, CancellationToken ct = default)
    {
        liveStreamManager.DisconnnectLiveStream(request.StreamKey);

        await liveStreamHub.NotifyStreamDisconnected(request.StreamKey);
        return Unit.Value;
    }
}
