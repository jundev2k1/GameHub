using game_x.application.Contract.Infrastructure.SignalR.Dtos;

namespace game_x.application.Features.Notification.Shared.Queries.GetNotificationDetail;

public record GetNotificationDetailQuery : IQuery<NotificationDto[]>;