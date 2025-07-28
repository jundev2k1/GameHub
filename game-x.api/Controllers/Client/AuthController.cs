using game_x.application.Features.Auth.Commands.Login.UserLogin;
using game_x.application.Features.Auth.Commands.Register.RegisterUser;

namespace game_x.api.Controllers.Client;

[AllowAnonymous]
[Route("api/user/auth")]
public sealed class AuthController : BaseApiController
{
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(UserLoginCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result, MessageCode.System.LoginSuccess);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterUserCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result, MessageCode.User.UserRegistSuccess);
    }
}
