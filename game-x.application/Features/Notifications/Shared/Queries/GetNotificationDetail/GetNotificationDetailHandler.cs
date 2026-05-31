using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Notifications.Dtos;

namespace game_x.application.Features.Notifications.Shared.Queries.GetNotificationDetail;

public sealed class GetNotificationDetailHandler(INotificationRepo notificationRepo, IUserAccessor userAccessor)
    : IQueryHandler<GetNotificationDetailQuery, NotificationListDto>
{
    public async Task<NotificationListDto> Handle(GetNotificationDetailQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var result = await notificationRepo
            .GetNotificationByUserIdAsync(userId, request.PageNo, request.PageSize, ct);
        return result;
    }
}
