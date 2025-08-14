using game_x.application.Contract.Infrastructure.SignalR.Dtos;

namespace game_x.application.Features.Notifications.Shared.Queries.GetNotificationDetail;

public record GetNotificationDetailQuery(
    int PageNo = 1,
    int PageSize = 20) : IQuery<NotificationDto[]>;
