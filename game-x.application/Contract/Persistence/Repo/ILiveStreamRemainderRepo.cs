namespace game_x.application.Contract.Persistence.Repo;

public interface ILiveStreamRemainderRepo
{
    Task<int> CountAllByStreamIdAsync(int streamId, CancellationToken ct = default);

    Task<LiveStreamReminder[]> GetAllByStreamIdAsync(int streamId, int offset, int count, CancellationToken ct = default);

    Task<LiveStreamReminder[]> GetAllPendingRemaindersAsync(string userId, CancellationToken ct = default);

    Task CreateAsync(LiveStreamReminder remainder, CancellationToken ct = default);

    Task CreateRangeAsync(IEnumerable<LiveStreamReminder> remainders, CancellationToken ct = default);

    Task MarkAsSentAsync(string userId, int streamId, NotificationChannel channel, CancellationToken ct = default);

    Task DeleteAsync(string userId, int streamId, NotificationChannel channel, CancellationToken ct = default);
}
