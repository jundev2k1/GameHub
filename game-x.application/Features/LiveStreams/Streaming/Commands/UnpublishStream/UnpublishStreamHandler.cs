using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Streaming.Commands.UnpublishStream;

public sealed class UnpublishStreamHandler(
    ILiveStreamManagerCacheService liveStreamManager,
    ILiveStreamHubService liveStreamHub,
    ILiveStreamRepo liveStreamRepo,
    IUnitOfWork unitOfWork) : ICommandHandler<UnpublishStreamCommand>
{
    public async Task<Unit> Handle(UnpublishStreamCommand request, CancellationToken ct = default)
    {
        liveStreamManager.DisconnnectLiveStream(request.StreamKey);

        await liveStreamRepo.UpdateAsync(request.StreamKey, async schedule =>
        {
            schedule.DisconnectStream();
            await unitOfWork.SaveChangesAsync(ct);
        }, ct);

        await liveStreamHub.NotifyStreamDisconnected(request.StreamKey);
        return Unit.Value;
    }
}
