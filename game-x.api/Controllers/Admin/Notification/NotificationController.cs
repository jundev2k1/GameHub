using game_x.application.Features.Notifications.Shared.Queries.GetNotificationDetail;

namespace game_x.api.Controllers.Admin.Notification;

[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin/notifications")]
public sealed class NotificationController : BaseApiController
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMyNotificationAsync(int page = 1, int pageSize = 20)
    {
        var result = await Mediator.Send(new GetNotificationDetailQuery(page, pageSize));
        return ApiResponseFactory.Ok(result);
    }
}
