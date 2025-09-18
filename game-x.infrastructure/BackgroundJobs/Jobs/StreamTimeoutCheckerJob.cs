using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Jobs;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.LiveStreams.Dtos;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.infrastructure.BackgroundJobs.Jobs;

public sealed class StreamTimeoutCheckerJob(
    ILiveStreamManagerCacheService liveStreamManager,
    IUnitOfWork unitOfWork,
    ILiveStreamRepo liveStreamRepo,
    IOptions<RecurringJobSettings> jobOptions,
    IAppLogger<StreamTimeoutCheckerJob> logger) : IRecurringJob
{
    public string JobId => "stream-timeout-checker";
    public string CronExpression => jobOptions.Value.StreamTimeoutCheckerJob;
    public bool IsInit => false;

    private const int StreamTimeoutMinutes = 10;

    public async Task ExecuteAsync(CancellationToken ct = default)
    {
        // Get all streams that are offline for more than StreamTimeoutMinutes
        var allStreams = liveStreamManager.GetAllStreamKeys()
            .Select(liveStreamManager.GetLiveStreamStatus)
            .Where(s => s is not null)
            .ToArray();
        var endedStreams = allStreams
            .Where(s => !s!.IsLive
                && s.EndTime > DateTime.UtcNow
                && s.OfflineAt.HasValue
                && (DateTime.UtcNow - s.OfflineAt.Value).Minutes > StreamTimeoutMinutes)
            .ToArray();
        if (endedStreams.Length == 0) return;

        foreach (var streamList in endedStreams.Chunk(500))
        {
            // Update stream status to ended
            (Guid Id, DateTime EndTime)[] streamInfos =
                [.. streamList.Select(si => (si!.Id, si.OfflineAt! ?? si.EndTime))];
            await unitOfWork.WithTransactionAsync(async () =>
            {
                await liveStreamRepo.BulkUpdateEndedStreams(streamInfos, ct);
            }, ct);

            // Cleanup cache
            CleanupCaches(streamList!);

            // Log info
            logger.LogInformation($"Stream timeout checker job ended {streamList.Length} streams.");
        }
    }

    private void CleanupCaches(LiveStreamStatusDto[] streams)
    {
        foreach (var stream in streams)
        {
            liveStreamManager.RemoveLiveStream(stream.StreamKey);
            liveStreamManager.RemoveViewersByStreamKey(stream.StreamKey);
        }
    }
}
