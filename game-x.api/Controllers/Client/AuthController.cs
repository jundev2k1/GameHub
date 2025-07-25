using game_x.application.Features.Auth.Commands.Login.UserLogin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public IActionResult RegisterAsync()
    {
        return ApiResponseFactory.Ok(MessageCode.User.UserRegistSuccess);
    }
}
