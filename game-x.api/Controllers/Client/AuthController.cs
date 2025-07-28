using game_x.application.Features.Auth.Commands.Login.UserLogin;
using game_x.application.Features.Auth.Commands.Register.RegisterUser;
using game_x.application.Features.Auth.Commands.ResendCode.ResendCodeUser;
using game_x.application.Features.Auth.Commands.Verify.VerifyEmailUser;
using game_x.application.Features.Auth.Client.Commands.UserLogin;
using game_x.application.Features.Auth.Client.Commands.Register.Client;

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
        return ApiResponseFactory.Ok(result, MessageCode.User.UserRegisterSuccess);
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmailAsync(VerifyEmailUserCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result, MessageCode.User.EmailVerifySuccess);
    }

    [HttpPost("resend-code")]
    public async Task<IActionResult> ResendCode(ResendCodeUserCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.Ok(MessageCode.System.EmailSendSuccess);
    }
}
