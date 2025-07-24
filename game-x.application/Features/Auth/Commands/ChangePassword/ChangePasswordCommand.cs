namespace game_x.application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommand : ICommand
{
    public required string Password { get; set; }
    public required string NewPassword { get; set; }
}