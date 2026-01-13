using game_x.application.Contract.Infrastructure.Email;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using System.Collections;
using System.Text.Json;

namespace game_x.application.Features.LiveStreams.Streaming.Jobs;

public sealed class SendRemindersJob(
    ILiveStreamReminderRepo streamReminderRepo,
    IUnitOfWork unitOfWork,
    INotificationRepo notificationRepo,
    IClientHubService clientHubService,
    IEmailService emailService,
    IAppLogger<SendRemindersJob> logger)
{
    public async Task ExecuteAsync(string streamKey, CancellationToken ct = default)
    {
        logger.LogInformation("[Job] SendRemindersJob for Stream ({StreamKey})", streamKey);
        var reminders = await streamReminderRepo
            .GetRemindersForNotificationByStreamKeyAsync(streamKey, ct);
        var reminderGroups = reminders.GroupBy(r => r.Channel);
        foreach (var group in reminderGroups)
        {
            switch (group.Key)
            {
                case NotificationChannel.Push:
                    await PushNotificationAsync(group, ct);
                    break;

                case NotificationChannel.Email:
                    await SendEmailAsync(group, ct);
                    break;

                case NotificationChannel.SMS:
                    await SendSmsAsync(group, ct);
                    break;
            }
        }
    }

    private async Task PushNotificationAsync(IEnumerable<LiveStreamReminder> reminders, CancellationToken ct)
    {
        if (!reminders.Any()) return;

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

        static Notification CreateNotification(LiveStreamReminder reminder)
        {
            var isStartStream = reminder.Schedule.Status == LiveStreamStatus.Live;
            return Notification.Create(
                isStartStream
                    ? NotificationMessageKey.LiveStream_Started
                    : NotificationMessageKey.LiveStream_ScheduleCancelled,
                reminder.UserId,
                NotificationType.LiveStream,
                isStartStream ? NotificationSeverity.Info : NotificationSeverity.Error,
                CreateMetadata(reminder));
        }

        IEnumerable<Notification> notifications = [];
        await unitOfWork.WithTransactionAsync(async () =>
        {
            var firstItem = reminders.FirstOrDefault();

            notifications = reminders.Select(CreateNotification);
            if (!notifications.Any()) return;

            // Create notifications in DB
            await notificationRepo.AddRangeNotificationsAsync(notifications, ct);

            // Mark as sent for all reminders in a live stream
            await streamReminderRepo.MarkAsSentsAsync(firstItem!.ScheduleId, NotificationChannel.Push, ct);
        }, ct);

        // Send notification for subscribers
        foreach (var notification in notifications)
        {
            await clientHubService.SendNotificationToMemberAsync(
                notification.UserId!,
                notification.Adapt<NotificationDto>());
        }
    }

    private async Task SendEmailAsync(IEnumerable<LiveStreamReminder> reminders, CancellationToken ct)
    {
        if (!reminders.Any()) return;

        var schedule = reminders.FirstOrDefault()!.Schedule;
        var isStartStream = schedule.Status == LiveStreamStatus.Live;

        await unitOfWork.WithTransactionAsync(async () =>
        {
            await streamReminderRepo.MarkAsSentsAsync(schedule.Id, NotificationChannel.Email, ct);
        }, ct);

        foreach (var reminder in reminders)
        {
            if (isStartStream)
            {
                await emailService.SendLiveStreamRemainderEmailAsync(
                    reminder.User.Email!,
                    reminder.Schedule);
            }
            else
            {
                await emailService.SendLiveStreamCancellationEmailAsync(
                    reminder.User.Email!,
                    reminder.Schedule);
            }
        }
    }

    private static async Task SendSmsAsync(IEnumerable<LiveStreamReminder> reminders, CancellationToken ct)
    {
        // TODO (LiveStreamRemainder): Add logic to send SMS for subscribers
        await Task.CompletedTask;
        _ = reminders;
        _ = ct;
    }
}
