using game_x.application.Features.Notifications.Shared.Queries.GetAdjacentNotifications;
using game_x.application.Features.Notifications.Shared.Queries.GetNotificationDetail;

namespace game_x.api.Controllers.Client.Notification;

[Authorize(Roles = AppRoles.User)]
[Route("/api/user/notifications")]
public sealed class NotificationController : BaseApiController
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMyNotificationAsync(int page = 1, int pageSize = 20)
    {
        var result = await Mediator.Send(new GetNotificationDetailQuery(page, pageSize));
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("me/{currentId:guid}/adjacent")]
    public async Task<IActionResult> GetAdjacentNotificationsAsync(Guid currentId, int pageSize, bool isNext = true)
    {
        var query = new GetAdjacentNotificationsQuery(currentId, pageSize, isNext);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}
