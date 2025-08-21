using game_x.application.Features.Auth.Root.Commands.RootLogin;

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
}
