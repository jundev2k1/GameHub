namespace game_x.application.Features.Auth.Client.Commands.ResetPassword;

public record ResetPasswordCommand(string OldPassword, string NewPassword) : ICommand;
