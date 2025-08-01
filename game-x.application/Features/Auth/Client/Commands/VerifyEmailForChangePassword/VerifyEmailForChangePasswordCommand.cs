namespace game_x.application.Features.Auth.Client.Commands.VerifyEmailForChangePassword;

public record VerifyEmailForChangePasswordCommand(
    string Email,
    string Code) : ICommand<VerifyEmailForPasswordResetResult>;

public record VerifyEmailForPasswordResetResult(string Token);
