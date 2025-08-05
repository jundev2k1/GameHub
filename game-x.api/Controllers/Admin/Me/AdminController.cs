using game_x.application.Features.Auth.Admin.Commands.ChangePasswordAdmin;

namespace game_x.api.Controllers.Admin.Me;

[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin")]
public sealed class AdminController : BaseApiController
{
    [HttpPatch("me/password")]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordAdminCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }
}
