namespace game_x.application.Features.Auth.Commands.ChangePassword;

public record ChangePasswordCommand(string Password, string NewPassword) : ICommand;
