using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Jobs;
using Hangfire;

namespace game_x.infrastructure.BackgroundJobs.Jobs;

public sealed class StreamViewRefresherJob(
    ILiveStreamManagerCacheService liveStreamManager,
    ILiveStreamHubService liveStreamSevice) : IRecurringJob
{
    public string JobId => "stream-viewer-refresher";
    public string? CronExpression => null;
    public bool IsInit => true;

    private const int JobRunAfterSeconds = 10;

    public async Task ExecuteAsync(CancellationToken ct = default)
    {
        // Get all streams that are offline for more than StreamTimeoutMinutes
        var allStreams = liveStreamManager.GetViewerChangeList();
        foreach (var streamKey in allStreams)
        {
            // Refresh view count for each live stream room
            var viewCount = liveStreamManager.GetViewerCount(streamKey);
            await liveStreamSevice.RefreshViewCount(streamKey, viewCount);
        }

        // Clean up caches
        liveStreamManager.CleanViewerChangeList();

        // Callback job after JobRunAfterSeconds seconds
        BackgroundJob.Schedule<StreamViewRefresherJob>(
            job => job.ExecuteAsync(CancellationToken.None),
            TimeSpan.FromSeconds(JobRunAfterSeconds));

        await Task.CompletedTask;
    }
}
