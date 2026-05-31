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
        int scheduleId,
        string userId,
        NotificationChannel channel = NotificationChannel.Push)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId is required.", nameof(userId));

        return new LiveStreamReminder
        {
            ScheduleId = scheduleId,
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
}
