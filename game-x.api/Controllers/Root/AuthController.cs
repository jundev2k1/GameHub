using game_x.application.Features.Auth.Client.Commands.RefreshToken;
using game_x.application.Features.Auth.Root.Commands.RootLogin;
using game_x.application.Features.Auth.Shared.Commands.Logout;

namespace game_x.api.Controllers.Root;

[Route("api/root/auth")]
public sealed class AuthController : BaseApiController
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(RootLoginCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result, MessageCode.System.LoginSuccess);
    }

    [Authorize(Roles = AppRoles.Root)]
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
