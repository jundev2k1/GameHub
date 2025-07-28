using game_x.application.Features.Auth.Admin.Commands.AdminLogin;

namespace game_x.api.Controllers.Admin;

[Route("api/admin/auth")]
public sealed class AuthController : BaseApiController
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(AdminLoginCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result, MessageCode.System.LoginSuccess);
    }
}
