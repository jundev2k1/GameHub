namespace game_x.application.Features.Auth.Shared.Commands.ChangePassword;

public record ChangePasswordCommand(string Password, string NewPassword) : ICommand;
