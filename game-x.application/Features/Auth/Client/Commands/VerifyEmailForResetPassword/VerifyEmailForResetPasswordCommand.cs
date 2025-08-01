namespace game_x.application.Features.Auth.Client.Commands.VerifyEmailForResetPassword;

public record VerifyEmailForResetPasswordCommand(
    string Email,
    string Code) : ICommand<VerifyEmailForPasswordResetResult>;

public record VerifyEmailForPasswordResetResult(string Token, DateTime ExpireTime);
