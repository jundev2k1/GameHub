using game_x.application.Contract.Infrastructure.SignalR.Dtos;

namespace game_x.application.Features.Notifications.Dtos;

public sealed class NotificationListDto
{
    public NotificationDto[] Items { get; set; } = [];
    public int TotalItems { get; set; }
    public int UnReadCount { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage { get; set; }
}
