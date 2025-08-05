using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class NotificationRepo(GameXContext context) : INotificationRepo, IRepository
{
    public async Task<Notification[]> GetNotificationByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var result = await context.Notifications
            .AsNoTracking()
            .Where(n => (n.UserId == null) || (n.UserId == userId))
            .ToArrayAsync(ct);
        return result;
    }

    public async Task<Notification> GetNotificationIdAsync(Guid notificationId, CancellationToken ct = default)
    {
        return await context.Notifications
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.PublicId == notificationId, ct)
            ?? throw new NotFoundException(nameof(notificationId), notificationId);
    }

    public async Task AddNotificationAsync(Notification notification, CancellationToken ct = default)
    {
        await context.Notifications.AddAsync(notification, ct);
    }

    public async Task MarkAsReadAsync(Guid notificationId, string userId, CancellationToken ct = default)
    {
        var targetNotification = await context.Notifications
            .FirstOrDefaultAsync(n => (n.PublicId == notificationId) && (n.UserId == userId), ct)
            ?? throw new NotFoundException(nameof(notificationId), notificationId);

        targetNotification.MarkAsRead();
    }

    public async Task DeleteNotificationAsync(Guid notificationId, string userId, CancellationToken ct = default)
    {
        var targetNotification = await context.Notifications
            .FirstOrDefaultAsync(n => (n.PublicId == notificationId) && (n.UserId == userId), ct)
            ?? throw new NotFoundException(nameof(notificationId), notificationId);

        context.Notifications.Remove(targetNotification);
    }
}
