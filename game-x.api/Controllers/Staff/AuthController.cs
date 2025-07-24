using game_x.application.Features.Auth.Commands.Login.StaffLogin;
using game_x.application.Features.Auth.Commands.Logout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Staff;

[Route("api/staff/auth")]
public class AuthController : BaseApiController
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(StaffLoginCommand command)
    {
        return ApiResponseFactory.Ok(await Mediator.Send(command), MessageCode.System.LoginSuccess);
    }

    [Authorize(Roles = AppRoles.Staff)]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(StaffLogoutCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(MessageCode.System.LogoutSuccess);
    }
}
