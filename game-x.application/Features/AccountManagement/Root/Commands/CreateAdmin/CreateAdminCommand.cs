namespace game_x.application.Features.AccountManagement.Root.Commands.CreateAdmin;

public sealed class CreateAdminCommand : ICommand
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
