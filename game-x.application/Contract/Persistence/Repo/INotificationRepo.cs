namespace game_x.application.Contract.Persistence.Repo;

public interface INotificationRepo
{
    Task<Notification[]> GetNotificationByUserIdAsync(
        string userId,
        int pageNo = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<Notification[]> GetAdjacentNotificationsAsync(
        string userId,
        Guid currentNotificationId,
        bool isNext = true,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<Notification> GetNotificationIdAsync(Guid notificationId, CancellationToken ct = default);

    Task AddNotificationAsync(Notification notification, CancellationToken ct = default);

    Task MarkAsReadAsync(Guid notificationCode, string userId, CancellationToken ct = default);

    Task DeleteNotificationAsync(Guid notificationCode, string userId, CancellationToken ct = default);
}
