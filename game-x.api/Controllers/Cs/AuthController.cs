using game_x.application.Features.Auth.Client.Commands.RefreshToken;
using game_x.application.Features.Auth.Cs.Commands.CsLogin;
using game_x.application.Features.Auth.Shared.Commands.Logout;

namespace game_x.api.Controllers.Cs;

[Route("api/cs/auth")]
public sealed class AuthController : BaseApiController
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(CsLoginCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result, MessageCode.System.LoginSuccess);
    }

    [Authorize(Roles = AppRoles.Cs)]
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        var command = new LogoutCommand();
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(MessageCode.System.LogoutSuccess);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshTokenAsync(RefreshTokenCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }
}
