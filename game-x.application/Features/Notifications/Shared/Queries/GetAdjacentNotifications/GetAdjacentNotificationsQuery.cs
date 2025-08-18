using game_x.application.Features.Notifications.Dtos;

namespace game_x.application.Features.Notifications.Shared.Queries.GetAdjacentNotifications;

public record GetAdjacentNotificationsQuery(Guid CurrentId, int PageSize, bool IsNext) : IQuery<NotificationListDto>;
