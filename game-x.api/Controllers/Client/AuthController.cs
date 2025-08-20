using game_x.api.Dtos;
using game_x.api.Enums;
using game_x.application.Exceptions;
using game_x.application.Features.Auth.Client.Commands.RefreshToken;
using game_x.application.Features.Auth.Client.Commands.RegisterUser;
using game_x.application.Features.Auth.Client.Commands.ResendCodeUser;
using game_x.application.Features.Auth.Client.Commands.ResetPasswordUser;
using game_x.application.Features.Auth.Client.Commands.UserLogin;
using game_x.application.Features.Auth.Client.Commands.UserLogout;
using game_x.application.Features.Auth.Client.Commands.VerifyEmailForChangePassword;
using game_x.application.Features.Auth.Client.Commands.VerifyEmailForRegistration;
using game_x.application.Features.Auth.Client.Commands.VerifyEmailForResetPassword;
using game_x.application.Services.Verification;

namespace game_x.api.Controllers.Client;

[Route("api/user/auth")]
public sealed class AuthController : BaseApiController
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(UserLoginCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result, MessageCode.System.LoginSuccess);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterUserCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result, MessageCode.User.UserRegisterSuccess);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshTokenAsync(RefreshTokenCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync(ResetPasswordUserCommand command)
    {
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result, MessageCode.User.UserResetPasswordSuccess);
    }

    [Authorize(Roles = AppRoles.User)]
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        var command = new UserLogoutCommand();
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(MessageCode.System.LogoutSuccess);
    }

    [AllowAnonymous]
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmailAsync(VerifyEmailRequest request)
    {
        if (request.Purpose == EmailVerificationPurpose.AccountActivation)
        {
            var result = await Mediator.Send(new VerifyEmailForRegistrationCommand(request.Email ?? string.Empty, request.Code));
            return ApiResponseFactory.Ok(result, MessageCode.User.EmailVerifySuccess);
        }

        if (request.Purpose == EmailVerificationPurpose.PasswordReset)
        {
            var result = await Mediator.Send(new VerifyEmailForResetPasswordCommand(request.Email ?? string.Empty, request.Code));
            return ApiResponseFactory.Ok(result, MessageCode.User.EmailVerifySuccess);
        }

        if (request.Purpose == EmailVerificationPurpose.PasswordChange)
        {
            if (request.Email != null)
                throw new BadRequestException("Change password does not need to be passing 'Email' parameter.");

            var result = await Mediator.Send(new VerifyEmailForChangePasswordCommand(request.Code));
            return ApiResponseFactory.Ok(result, MessageCode.User.EmailVerifySuccess);
        }

        throw new BadRequestException(MessageCode.System.InvalidParameters);
    }

    [AllowAnonymous]
    [HttpPost("resend-code")]
    public async Task<IActionResult> ResendCodeAsync(ResendCodeRequest request)
    {
        var purpose = request.Purpose switch
        {
            EmailVerificationPurpose.AccountActivation => VerificationPurposes.EmailVerification,
            EmailVerificationPurpose.PasswordReset => VerificationPurposes.ForgotPassword,
            EmailVerificationPurpose.PasswordChange => VerificationPurposes.ChangePassword,
            _ => throw new BadRequestException(),
        };
        var command = request.Adapt<ResendCodeUserCommand>() with { Purpose = purpose };
        await Mediator.Send(command);
        return ApiResponseFactory.Ok(MessageCode.System.EmailSendSuccess);
    }
}
