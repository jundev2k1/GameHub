using game_x.application.Features.Auth.Client.Commands.RegisterUser;
using game_x.application.Features.Auth.Client.Commands.ResendCodeUser;
using game_x.application.Features.Auth.Client.Commands.UserLogin;
using game_x.application.Features.Auth.Client.Commands.VerifyEmailForRegistration;
using game_x.application.Features.Auth.Client.Commands.VerifyEmailForChangePassword;
using game_x.api.Dtos;
using game_x.api.Enums;
using game_x.application.Exceptions;
using game_x.application.Services.Verification;
using game_x.application.Features.Auth.Client.Commands.VerifyEmailForResetPassword;

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
    public async Task<IActionResult> VerifyEmailAsync(VerifyEmailRequest request)
    {
        if (request.Purpose == EmailVerificationPurpose.AccountActivation)
        {
            var result = await Mediator.Send(new VerifyEmailForRegistrationCommand(request.Email, request.Code));
            return ApiResponseFactory.Ok(result, MessageCode.User.EmailVerifySuccess);
        }

        if (request.Purpose == EmailVerificationPurpose.PasswordReset)
        {
            var result = await Mediator.Send(new VerifyEmailForResetPasswordCommand(request.Email, request.Code));
            return ApiResponseFactory.Ok(result, MessageCode.User.EmailVerifySuccess);
        }

        if (request.Purpose == EmailVerificationPurpose.ChangePassword)
        {
            if (request.Email != null)
                throw new BadRequestException("Change password does not need to be passing 'Email' parameter.");

            var result = await Mediator.Send(new VerifyEmailForChangePasswordCommand(request.Code));
            return ApiResponseFactory.Ok(result, MessageCode.User.EmailVerifySuccess);
        }

        throw new BadRequestException(MessageCode.System.InvalidParameters);
    }

    [HttpPost("resend-code")]
    public async Task<IActionResult> ResendCode(ResendCodeRequest request)
    {
        var purpose = request.Purpose switch
        {
            EmailVerificationPurpose.AccountActivation => VerificationPurposes.EmailVerification,
            EmailVerificationPurpose.PasswordReset => VerificationPurposes.ForgotPassword,
            EmailVerificationPurpose.ChangePassword => VerificationPurposes.ChangePassword,
            _ => throw new BadRequestException(),
        };
        var command = request.Adapt<ResendCodeUserCommand>() with { Purpose = purpose };
        await Mediator.Send(command);
        return ApiResponseFactory.Ok(MessageCode.System.EmailSendSuccess);
    }
}
