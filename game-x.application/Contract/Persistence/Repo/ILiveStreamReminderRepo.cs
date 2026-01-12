namespace game_x.application.Contract.Persistence.Repo;

public interface ILiveStreamReminderRepo
{
    Task<int> CountAllByStreamIdAsync(int streamId, CancellationToken ct = default);

    Task<LiveStreamReminder[]> GetAllByStreamIdAsync(int streamId, int offset, int count, CancellationToken ct = default);

    Task<LiveStreamReminder[]> GetAllPendingRemindersAsync(string userId, CancellationToken ct = default);

    Task<LiveStreamReminder[]> GetRemindersForNotificationByStreamKeyAsync(string streamKey, CancellationToken ct = default);

    Task CreateAsync(LiveStreamReminder reminder, CancellationToken ct = default);

    Task CreateRangeAsync(IEnumerable<LiveStreamReminder> reminders, CancellationToken ct = default);

    Task MarkAsSentAsync(string userId, int streamId, NotificationChannel channel, CancellationToken ct = default);

    Task MarkAsSentsAsync(int streamId, NotificationChannel channel, CancellationToken ct = default);

    Task DeleteAsync(string userId, int streamId, CancellationToken ct = default);
    Task DeleteAsync(string userId, int streamId, NotificationChannel channel, CancellationToken ct = default);
}
