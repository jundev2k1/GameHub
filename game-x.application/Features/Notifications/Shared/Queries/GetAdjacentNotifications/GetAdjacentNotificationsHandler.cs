using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Notifications.Shared.Queries.GetAdjacentNotifications;

public sealed class GetAdjacentNotificationsHandler(
    IUserAccessor userAccessor,
    INotificationRepo notificationRepo) : IQueryHandler<GetAdjacentNotificationsQuery, NotificationDto[]>
{
    public async Task<NotificationDto[]> Handle(GetAdjacentNotificationsQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var result = await notificationRepo
            .GetAdjacentNotificationsAsync(userId, request.CurrentId, request.IsNext, request.PageSize, ct);
        return [.. result.Select(n => n.Adapt<NotificationDto>())];
    }
}
