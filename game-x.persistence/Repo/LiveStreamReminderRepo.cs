using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class LiveStreamReminderRepo(GameXContext dbContext) : ILiveStreamReminderRepo, IRepository
{
    public async Task<int> CountAllByStreamIdAsync(int streamId, CancellationToken ct = default)
    {
        return await dbContext.LiveStreamReminders
            .AsNoTracking()
            .CountAsync(lr => lr.ScheduleId == streamId && lr.Status == ReminderStatus.Pending, ct);
    }

    public async Task<LiveStreamReminder[]> GetAllByStreamIdAsync(
        int streamId,
        int offset,
        int count,
        CancellationToken ct = default)
    {
        return await dbContext.LiveStreamReminders
            .AsNoTracking()
            .Where(lr => lr.ScheduleId == streamId && lr.Status == ReminderStatus.Pending)
            .Skip(offset)
            .Take(count)
            .ToArrayAsync(ct);
    }

    public async Task<LiveStreamReminder[]> GetAllPendingRemindersAsync(string userId, CancellationToken ct = default)
    {
        return await dbContext.LiveStreamReminders
            .AsNoTracking()
            .Include(lr => lr.Schedule)
            .Include(lr => lr.User)
            .Where(lr => lr.UserId == userId && lr.Status == ReminderStatus.Pending)
            .ToArrayAsync(ct);
    }

    public async Task<LiveStreamReminder[]> GetRemindersForNotificationByStreamKeyAsync(string streamKey, CancellationToken ct = default)
    {
        LiveStreamStatus[] validStatuses = [LiveStreamStatus.Scheduled, LiveStreamStatus.Cancelled];
        return await dbContext.LiveStreamReminders
            .AsNoTracking()
            .Include(lsr => lsr.User)
            .Include(lsr => lsr.Schedule)
            .Where(lsr => lsr.Schedule.StreamKey == streamKey
                && validStatuses.Contains(lsr.Schedule.Status)
                && lsr.Status == ReminderStatus.Pending)
            .ToArrayAsync(ct);
    }

    public async Task<Dictionary<Guid, NotificationChannel[]>> GetStreamRemindersAsync(
        IEnumerable<Guid> ids,
        CancellationToken ct = default)
    {
        return await dbContext.LiveStreamReminders
            .AsNoTracking()
            .Where(lsr => ids.Contains(lsr.Schedule.PublicId))
            .GroupBy(lsr => lsr.Schedule.PublicId)
            .ToDictionaryAsync(gr => gr.Key, gr => gr.Select(lsr => lsr.Channel).ToArray(), ct);
    }

    public async Task CreateAsync(LiveStreamReminder reminder, CancellationToken ct = default)
    {
        await dbContext.LiveStreamReminders.AddAsync(reminder, ct);
    }

    public async Task CreateRangeAsync(IEnumerable<LiveStreamReminder> reminders, CancellationToken ct = default)
    {
        await dbContext.LiveStreamReminders.AddRangeAsync(reminders, ct);
    }

    public async Task MarkAsSentAsync(string userId, int streamId, NotificationChannel channel, CancellationToken ct = default)
    {
        var target = await dbContext.LiveStreamReminders
            .FirstOrDefaultAsync(lr => (lr.UserId == userId)
                && (lr.ScheduleId == streamId)
                && (lr.Channel == channel), ct)
            ?? throw new NotFoundException("Target Reminder was not found.");

        target.MarkAsSent();
    }

    public async Task MarkAsSentsAsync(int streamId, NotificationChannel channel, CancellationToken ct = default)
    {
        await dbContext.LiveStreamReminders
            .Where(lr => lr.ScheduleId == streamId && lr.Channel == channel)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(lr => lr.Status, ReminderStatus.Sent)
                .SetProperty(lr => lr.SentAt, DateTime.UtcNow), ct);
    }

    public async Task DeleteAsync(string userId, int streamId, CancellationToken ct = default)
    {
        await dbContext.LiveStreamReminders
            .Where(lr => (lr.UserId == userId)
                && (lr.ScheduleId == streamId))
            .ExecuteDeleteAsync(ct);
    }

    public async Task DeleteAsync(string userId, int streamId, NotificationChannel channel, CancellationToken ct = default)
    {
        await dbContext.LiveStreamReminders
            .Where(lr => (lr.UserId == userId)
                && (lr.ScheduleId == streamId)
                && (lr.Channel == channel))
            .ExecuteDeleteAsync(ct);
    }
}
