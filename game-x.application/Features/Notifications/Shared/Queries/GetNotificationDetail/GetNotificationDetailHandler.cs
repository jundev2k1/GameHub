using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Notifications.Shared.Queries.GetNotificationDetail;

public sealed class GetNotificationDetailHandler(INotificationRepo notificationRepo, IUserAccessor userAccessor)
    : IQueryHandler<GetNotificationDetailQuery, NotificationDto[]>
{
    public async Task<NotificationDto[]> Handle(GetNotificationDetailQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var pageSize = request.PageSize > 0 && request.PageSize <= 500
            ? request.PageSize
            : 10;
        var notifications = await notificationRepo
            .GetNotificationByUserIdAsync(userId, pageSize, ct);

        var result = notifications
            .Select(n => n.Adapt<NotificationDto>())
            .ToArray();
        return result;
    }
}
