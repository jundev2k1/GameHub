namespace game_x.application.Features.Auth.Client.Commands.VerifyEmailForChangePassword;

public record VerifyEmailForChangePasswordCommand(string Code) : ICommand<VerifyEmailForChangePasswordResult>;

public record VerifyEmailForChangePasswordResult(string Token, DateTime ExpireTime);
