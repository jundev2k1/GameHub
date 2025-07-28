using game_x.application.Features.Auth.Cs.Commands.CsLogin;

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
}
