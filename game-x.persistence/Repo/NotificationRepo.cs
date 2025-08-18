using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.Notifications.Dtos;
using Mapster;

namespace game_x.persistence.Repo;

public sealed class NotificationRepo(GameXContext context, IUnitOfWork unitOfWork) : INotificationRepo, IRepository
{
    public async Task<Notification[]> GetNotificationByUserIdAsync(
        string userId,
        int pageNo = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var beginCount = (pageNo - 1) * pageSize;
        var result = await context.Notifications
            .AsNoTracking()
            .Where(n => (n.UserId == null) || (n.UserId == userId))
            .OrderByDescending(n => n.CreatedAt)
            .Skip(beginCount)
            .Take(pageSize)
            .ToArrayAsync(ct);
        return result;
    }

    public async Task<NotificationListDto> GetAdjacentNotificationsAsync(
        string userId,
        Guid currentNotificationId,
        bool isNext = true,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var targetNotification = await context.Notifications
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.UserId == userId && n.PublicId == currentNotificationId, ct)
            ?? throw new NotFoundException(nameof(Notification), currentNotificationId);

        var dateTime = targetNotification.CreatedAt;
        var notifications = context.Notifications
            .AsNoTracking()
            .AsQueryable()
            .Where(n => n.UserId == userId);
        var totalCount = await notifications.CountAsync(ct);
        var unReadCount = await notifications.CountAsync(n => n.IsRead == false, ct);
        var data = notifications
            .Where(n => isNext ? n.CreatedAt < dateTime : n.CreatedAt > dateTime);
        data = isNext
            ? data.OrderByDescending(n => n.CreatedAt)
            : data.OrderBy(n => n.CreatedAt);
        var hasNextPage = await data.Skip(pageSize).AnyAsync(ct);
        var items = await data.Take(pageSize).ToArrayAsync(ct);
        return new()
        {
            Items = [.. items.Select(n => n.Adapt<NotificationDto>()).OrderByDescending(n => n.CreatedAt)],
            TotalItems = totalCount,
            UnReadCount = unReadCount,
            PageSize = pageSize,
            HasNextPage = hasNextPage,
        };
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

    public async Task MarkAllAsReadAsync(string userId, CancellationToken ct = default)
    {
        var batchSize = 100;
        int skip = 0;
        List<Notification> batch;

        do
        {
            batch = await context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderBy(n => n.Id)
                .Skip(skip)
                .Take(batchSize)
                .ToListAsync(ct);

            if (batch.Count > 0)
            {
                await unitOfWork.BeginTransactionAsync(ct);

                try
                {
                    foreach (var notification in batch)
                    {
                        notification.MarkAsRead();
                    }

                    await context.SaveChangesAsync(ct);
                    await unitOfWork.CommitAsync(ct);
                }
                catch
                {
                    await unitOfWork.RollbackAsync(ct);
                    throw;
                }
            }

            skip += batchSize;
        }
        while (batch.Count == batchSize);
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
