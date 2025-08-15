using game_x.application.Contract.Infrastructure.SignalR.Dtos;

namespace game_x.application.Features.Notifications.Shared.Queries.GetAdjacentNotifications;

public record GetAdjacentNotificationsQuery(Guid CurrentId, int PageSize, bool IsNext) : IQuery<NotificationDto[]>;
