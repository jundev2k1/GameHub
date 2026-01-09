namespace game_x.domain.Entities;

public sealed class LiveStreamReminder : BaseEntity<int>
{
    public int ScheduleId { get; private set; }
    public LivestreamSchedule Schedule { get; private set; } = default!;
    public string UserId { get; private set; } = string.Empty;
    public User User { get; private set; } = default!;
    public ReminderStatus Status { get; private set; }
    public DateTime? SentAt { get; private set; }
    public NotificationChannel Channel { get; private set; }

    public static LiveStreamReminder Create(
        LivestreamSchedule schedule,
        string userId,
        NotificationChannel channel = NotificationChannel.Push)
    {
        ArgumentNullException.ThrowIfNull(schedule);
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId is required.", nameof(userId));

        return new LiveStreamReminder
        {
            ScheduleId = schedule.Id,
            UserId = userId,
            Channel = channel,
            Status = ReminderStatus.Pending,
        };
    }

    public void MarkAsSent()
    {
        if (Status != ReminderStatus.Pending)
            throw new InvalidOperationException("Only pending reminders can be sent.");

        Status = ReminderStatus.Sent;
        SentAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string reason)
    {
        if (reason.IsNullOrWhiteSpace())
            throw new ArgumentException("Failure reason is required.", nameof(reason));

        Status = ReminderStatus.Failed;
    }

    public void Cancel()
    {
        if (Status == ReminderStatus.Sent)
            throw new InvalidOperationException("Sent reminders cannot be canceled.");

        Status = ReminderStatus.Canceled;
    }
}
