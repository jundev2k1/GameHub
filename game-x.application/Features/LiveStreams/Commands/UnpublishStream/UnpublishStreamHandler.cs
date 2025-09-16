using game_x.application.Contract.Infrastructure.Caching;

namespace game_x.application.Features.LiveStreams.Commands.UnpublishStream;

public sealed class UnpublishStreamHandler(
    ILiveStreamManagerCacheService liveStreamManager) : ICommandHandler<UnpublishStreamCommand>
{
    public async Task<Unit> Handle(UnpublishStreamCommand request, CancellationToken ct = default)
    {
        liveStreamManager.DisconnnectLiveStream(request.StreamKey);

        await Task.CompletedTask;
        return Unit.Value;
    }
}
