using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Notifications.Dtos;

namespace game_x.application.Features.Notifications.Shared.Queries.GetAdjacentNotifications;

public sealed class GetAdjacentNotificationsHandler(
    IUserAccessor userAccessor,
    INotificationRepo notificationRepo) : IQueryHandler<GetAdjacentNotificationsQuery, NotificationListDto>
{
    public async Task<NotificationListDto> Handle(GetAdjacentNotificationsQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var result = await notificationRepo
            .GetAdjacentNotificationsAsync(userId, request.CurrentId, request.IsNext, request.PageSize, ct);
        return result;
    }
}
