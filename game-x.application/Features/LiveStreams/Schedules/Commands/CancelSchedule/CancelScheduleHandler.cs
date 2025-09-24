using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.Srs;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Schedules.Commands.CancelSchedule;

public sealed class CancelScheduleHandler(
    IUnitOfWork unitOfWork,
    ILiveStreamRepo liveStreamRepo,
    ILiveStreamManagerCacheService liveStreamManager,
    ISrsService srsService,
    ILiveStreamHubService liveStreamHub) : ICommandHandler<CancelScheduleCommand>
{
    public async Task<Unit> Handle(CancelScheduleCommand request, CancellationToken ct = default)
    {
        var streamKey = string.Empty;
        await liveStreamRepo.UpdateAsync(request.Id, async liveStream =>
        {
            liveStream.CancelStream(request.Reason);
            await unitOfWork.SaveChangesAsync(ct);
            streamKey = liveStream.StreamKey;
        }, ct);

        await StopStream(streamKey);

        // Notify all viewers and host
        await liveStreamHub.NotifyCancelStream(streamKey, request.Reason);
        return Unit.Value;
    }

    private async Task StopStream(string streamKey)
    {
        // Update the stream status in cache
        var streamInfo = liveStreamManager.GetLiveStreamStatus(streamKey);
        if (streamInfo is null) return;

        // Stop the stream in SRS
        await srsService.KickClientAsync(streamInfo.ClientId);

        // Remove the stream from cache
        liveStreamManager.RemoveLiveStream(streamKey);

        // Kick all viewers
        var viewers = liveStreamManager.GetAllViewersByStreamKey(streamInfo.StreamKey)
            .SelectMany(v => v.Value)
            .Select(token => liveStreamManager.GetViewerInfo(streamInfo.StreamKey, token))
            .ToArray();
        foreach (var viewer in viewers)
        {
            if (viewer is null) continue;

            await srsService.KickClientAsync(viewer.ClientId);
        }
    }
}
