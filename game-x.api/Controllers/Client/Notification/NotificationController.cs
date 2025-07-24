using game_x.application.Features.Notifications.Shared.Queries.GetNotificationDetail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
