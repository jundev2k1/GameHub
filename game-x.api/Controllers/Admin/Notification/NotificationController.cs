using game_x.application.Features.Notification.Shared.Queries.GetNotificationDetail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Admin.Notification;

[Authorize(Roles = AppRoles.Admin)]
[Route("/api/admin/notifications")]
public class NotificationController : BaseApiController
{
    [HttpGet("me")]
    public async Task<IActionResult> GetMyNotificationAsync()
    {
        var result = await Mediator.Send(new GetNotificationDetailQuery());
        return ApiResponseFactory.Ok(result);
    }
}
