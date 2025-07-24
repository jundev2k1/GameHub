namespace game_x.application.Features.AccountManagement.Admin.Commands.SoftDeleteUser;

public sealed class SoftDeleteUserCommand : ICommand
{
    public string UserId { get; set; } = string.Empty;
}
