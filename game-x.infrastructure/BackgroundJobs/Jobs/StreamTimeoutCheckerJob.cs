using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Jobs;
using game_x.application.Contract.Persistence.Repo;
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
        var expiredStreams = await liveStreamRepo.GetExpiredStreams(ct);
        logger.LogInformation($"Expired streams: {expiredStreams.Length}");

        // Get all active streams from cache which are live or offline less than timeout minutes
        var allActiveStreams = liveStreamManager.GetAllStreamKeys()
            .Select(liveStreamManager.GetLiveStreamStatus)
            .Where(s => s is not null
                && s.EndTime > DateTime.UtcNow
                && (s.IsLive
                    || (s.OfflineAt.HasValue && (DateTime.UtcNow - s.OfflineAt.Value).Minutes < StreamTimeoutMinutes)))
            .Select(s => s!.Id)
            .ToArray();
        var endedStreams = expiredStreams
            .Where(s => !allActiveStreams.Contains(s.PublicId))
            .ToArray();
        if (endedStreams.Length == 0) return;

        foreach (var streamList in endedStreams.Chunk(500))
        {
            // Update stream status to ended
            var streamIds = streamList.Select(s => s.PublicId).ToArray();
            await unitOfWork.WithTransactionAsync(async () =>
            {
                await liveStreamRepo.BulkUpdateEndedStreams(streamIds, ct);
            }, ct);

            // Cleanup cache
            CleanupCaches([.. streamList.Select(s => s.StreamKey)]);

            // Log info
            logger.LogInformation($"Stream timeout checker job ended {streamList.Length} streams.");
        }
    }

    private void CleanupCaches(string[] streamKeys)
    {
        foreach (var streamKey in streamKeys)
        {
            liveStreamManager.RemoveLiveStream(streamKey);
            liveStreamManager.RemoveViewersByStreamKey(streamKey);
        }
    }
}
