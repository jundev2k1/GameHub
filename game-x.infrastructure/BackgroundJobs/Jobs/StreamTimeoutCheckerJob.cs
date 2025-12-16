using FluentValidation;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Jobs;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.Settings;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace game_x.infrastructure.BackgroundJobs.Jobs;

public sealed class StreamTimeoutCheckerJob(
    ILiveStreamManagerCacheService liveStreamManager,
    IUnitOfWork unitOfWork,
    ILiveStreamRepo liveStreamRepo,
    ILiveStreamHubService liveStreamHub,
    INotificationRepo notificationRepo,
    IClientHubService clientHub,
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
            .SelectMany(kvp => kvp.Value)
            .Select(liveStreamManager.GetLiveStreamStatus)
            .Where(s => s is not null
                && s.EndTime < DateTime.UtcNow
                && (s.IsLive
                    || (!s.IsLive && (s.OfflineAt.HasValue && (DateTime.UtcNow - s.OfflineAt.Value).Minutes < StreamTimeoutMinutes))))
            .Select(s => s!.StreamKey)
            .ToArray();

        var endedStreams = expiredStreams
            .Where(s => !allActiveStreams.Contains(s.StreamKey))
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

            // Cleanup cache and notify talent
            await CleanupCacheAndNotifyForTalent(streamList, ct);

            // Log info
            logger.LogInformation($"Stream timeout checker job ended {streamList.Length} streams.");
        }
    }

    private async Task CleanupCacheAndNotifyForTalent(LivestreamSchedule[] streamInfos, CancellationToken ct)
    {
        foreach (var streamInfo in streamInfos)
        {
            liveStreamManager.RemoveLiveStream(streamInfo.StreamKey);
            liveStreamManager.RemoveViewersByStreamKey(streamInfo.StreamKey);

            // Notify talent and viewers if live
            if (streamInfo.Status == LiveStreamStatus.Live)
            {
                await liveStreamHub.NotifyEndStream(streamInfo.StreamKey);

                var message = new Dictionary<string, object>
                {
                    { "Title", streamInfo.Title }
                };
                await NotifyForTalent(
                    streamInfo.AssignedId!,
                    NotificationMessageKey.LiveStream_Ended,
                    JsonSerializer.Serialize(message),
                    NotificationSeverity.Info,
                    ct);
            }

            // Notify talent for timeout cancelled
            if (streamInfo.Status == LiveStreamStatus.Scheduled)
            {
                var message = new Dictionary<string, object>
                {
                    { "Title", streamInfo.Title },
                    { "Message", "Stream cancelled due to timeout." }
                };
                await NotifyForTalent(
                    streamInfo.AssignedId!,
                    NotificationMessageKey.LiveStream_TimeoutCancelled,
                    JsonSerializer.Serialize(message),
                    NotificationSeverity.Error,
                    ct);
            }
        }
    }

    private async Task NotifyForTalent(
        string userId,
        NotificationMessageKey message,
        string? detail = null,
        NotificationSeverity severity = NotificationSeverity.Info,
        CancellationToken ct = default)
    {
        var notification = Notification.Create(
            message,
            userId,
            NotificationType.System,
            severity,
            detail);
        await notificationRepo.AddNotificationAsync(notification, ct);
        await unitOfWork.SaveChangesAsync(ct);

        await clientHub.SendNotificationToMemberAsync(
            userId,
            notification.Adapt<NotificationDto>());
    }
}
