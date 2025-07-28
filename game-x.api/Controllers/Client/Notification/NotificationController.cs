using game_x.application.Features.Notifications.Shared.Queries.GetNotificationDetail;

namespace game_x.api.Controllers.Client.Notification;

[Authorize(Roles = AppRoles.User)]
[Route("/api/user/notifications")]
public sealed class NotificationController : BaseApiController
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMyNotificationAsync()
    {
        var result = await Mediator.Send(new GetNotificationDetailQuery());
        return ApiResponseFactory.Ok(result);
    }
}
