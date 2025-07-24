using game_x.application.Contract.Infrastructure.SignalR.Dtos;

namespace game_x.application.Features.Notifications.Shared.Queries.GetNotificationDetail;

public record GetNotificationDetailQuery : IQuery<NotificationDto[]>;
