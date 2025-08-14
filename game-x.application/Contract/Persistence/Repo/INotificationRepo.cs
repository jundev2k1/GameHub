namespace game_x.application.Contract.Persistence.Repo;

public interface INotificationRepo
{
    Task<Notification[]> GetNotificationByUserIdAsync(string userId, int pageSize = 20, CancellationToken ct = default);

    Task<Notification> GetNotificationIdAsync(Guid notificationId, CancellationToken ct = default);

    Task AddNotificationAsync(Notification notification, CancellationToken ct = default);

    Task MarkAsReadAsync(Guid notificationCode, string userId, CancellationToken ct = default);

    Task DeleteNotificationAsync(Guid notificationCode, string userId, CancellationToken ct = default);
}
