using game_x.application.Contract.Infrastructure.Email;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using System.Collections;
using System.Text.Json;

namespace game_x.application.Features.LiveStreams.Streaming.Jobs;

public sealed class SendRemaindersJob(
    ILiveStreamRemainderRepo streamRemainderRepo,
    IUnitOfWork unitOfWork,
    INotificationRepo notificationRepo,
    IClientHubService clientHubService,
    IEmailService emailService,
    IAppLogger<SendRemaindersJob> logger)
{
    public async Task ExecuteAsync(string streamKey, CancellationToken ct = default)
    {
        logger.LogInformation("[Job] SendRemaindersJob for Stream ({StreamKey})", streamKey);
        var remainders = await streamRemainderRepo
            .GetRemaindersForNotificationByStreamKeyAsync(streamKey, ct);
        var remainderGroups = remainders.GroupBy(r => r.Channel);
        foreach (var group in remainderGroups)
        {
            switch (group.Key)
            {
                case NotificationChannel.Push:
                    await PushNotificationAsync(remainders, ct);
                    break;

                case NotificationChannel.Email:
                    await SendEmailAsync(remainders, ct);
                    break;

                case NotificationChannel.SMS:
                    await SendSmsAsync(remainders, ct);
                    break;
            }
        }
    }

    private async Task PushNotificationAsync(IEnumerable<LiveStreamReminder> remainders, CancellationToken ct)
    {
        static string CreateMetadata(LiveStreamReminder reminder)
        {
            var metadata = new Hashtable
            {
                { "title", reminder.Schedule.Title },
                { "startTime", reminder.Schedule.StartTime },
                { "endTime", reminder.Schedule.EndTime },
                { "streamKey", reminder.Schedule.StreamKey },
            };
            return JsonSerializer.Serialize(metadata);
        }

        IEnumerable<Notification> notifications = [];
        await unitOfWork.WithTransactionAsync(async () =>
        {
            notifications = remainders
                .Select(r => Notification.Create(
                    NotificationMessageKey.LiveStream_Started,
                    r.UserId,
                    NotificationType.Info,
                    NotificationSeverity.Info,
                    CreateMetadata(r)));
            if (!notifications.Any()) return;

            // Create notifications in DB
            await notificationRepo.AddRangeNotificationsAsync(notifications, ct);

            // Mark as sent for all remainders in a live stream
            var firstItem = remainders.FirstOrDefault();
            await streamRemainderRepo.MarkAsSentsAsync(firstItem!.ScheduleId, NotificationChannel.Push, ct);
        }, ct);

        // Send notification for subscribers
        foreach (var notification in notifications)
        {
            await clientHubService.SendNotificationToMemberAsync(
                notification.UserId!,
                notification.Adapt<NotificationDto>());
        }
    }

    private async Task SendEmailAsync(IEnumerable<LiveStreamReminder> remainders, CancellationToken ct)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            foreach (var remainder in remainders)
            {
                await streamRemainderRepo.MarkAsSentAsync(
                    remainder.UserId,
                    remainder.Schedule.Id,
                    remainder.Channel,
                    ct);
                await emailService.SendLiveStreamRemainderEmailAsync(
                    remainder.User.Email!,
                    remainder.Schedule);
            }
        }, ct);
    }

    private static async Task SendSmsAsync(IEnumerable<LiveStreamReminder> remainders, CancellationToken ct)
    {
        // TODO (LiveStreamRemainder): Add logic to send SMS for subscribers
        await Task.CompletedTask;
        _ = remainders;
        _ = ct;
    }
}
