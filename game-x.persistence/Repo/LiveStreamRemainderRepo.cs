using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class LiveStreamRemainderRepo(GameXContext dbContext) : ILiveStreamRemainderRepo, IRepository
{
    public async Task<int> CountAllByStreamIdAsync(int streamId, CancellationToken ct = default)
    {
        return await dbContext.LiveStreamRemainders
            .AsNoTracking()
            .CountAsync(lr => lr.ScheduleId == streamId && lr.Status == ReminderStatus.Pending, ct);
    }

    public async Task<LiveStreamReminder[]> GetAllByStreamIdAsync(
        int streamId,
        int offset,
        int count,
        CancellationToken ct = default)
    {
        return await dbContext.LiveStreamRemainders
            .AsNoTracking()
            .Where(lr => lr.ScheduleId == streamId && lr.Status == ReminderStatus.Pending)
            .Skip(offset)
            .Take(count)
            .ToArrayAsync(ct);
    }

    public async Task<LiveStreamReminder[]> GetAllPendingRemaindersAsync(string userId, CancellationToken ct = default)
    {
        return await dbContext.LiveStreamRemainders
            .AsNoTracking()
            .Include(lr => lr.Schedule)
            .Include(lr => lr.User)
            .Where(lr => lr.UserId == userId && lr.Status == ReminderStatus.Pending)
            .ToArrayAsync(ct);
    }

    public async Task<LiveStreamReminder[]> GetRemaindersForNotificationByStreamKeyAsync(string streamKey, CancellationToken ct = default)
    {
        return await dbContext.LiveStreamRemainders
            .AsNoTracking()
            .Include(lsr => lsr.User)
            .Include(lsr => lsr.Schedule)
            .Where(lsr => lsr.Schedule.StreamKey == streamKey && lsr.Schedule.Status == LiveStreamStatus.Scheduled)
            .ToArrayAsync(ct);
    }

    public async Task CreateAsync(LiveStreamReminder remainder, CancellationToken ct = default)
    {
        await dbContext.LiveStreamRemainders.AddAsync(remainder, ct);
    }

    public async Task CreateRangeAsync(IEnumerable<LiveStreamReminder> remainders, CancellationToken ct = default)
    {
        await dbContext.LiveStreamRemainders.AddRangeAsync(remainders, ct);
    }

    public async Task MarkAsSentAsync(string userId, int streamId, NotificationChannel channel, CancellationToken ct = default)
    {
        var target = await dbContext.LiveStreamRemainders
            .FirstOrDefaultAsync(lr => (lr.UserId == userId)
                && (lr.ScheduleId == streamId)
                && (lr.Channel == channel), ct)
            ?? throw new NotFoundException("Target Remainder was not found.");

        target.MarkAsSent();
    }

    public async Task MarkAsSentsAsync(int streamId, NotificationChannel channel, CancellationToken ct = default)
    {
        await dbContext.LiveStreamRemainders
            .Where(lr => lr.ScheduleId == streamId && lr.Channel == channel)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(lr => lr.Status, ReminderStatus.Sent)
                .SetProperty(lr => lr.SentAt, DateTime.UtcNow), ct);
    }

    public async Task DeleteAsync(string userId, int streamId, CancellationToken ct = default)
    {
        await dbContext.LiveStreamRemainders
            .Where(lr => (lr.UserId == userId)
                && (lr.ScheduleId == streamId))
            .ExecuteDeleteAsync(ct);
    }

    public async Task DeleteAsync(string userId, int streamId, NotificationChannel channel, CancellationToken ct = default)
    {
        await dbContext.LiveStreamRemainders
            .Where(lr => (lr.UserId == userId)
                && (lr.ScheduleId == streamId)
                && (lr.Channel == channel))
            .ExecuteDeleteAsync(ct);
    }
}
