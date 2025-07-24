namespace game_x.application.Features.AccountManagement.Admin.Commands.UpdateUserStatusByAdmin;

public sealed class UpdateUserStatusByAdminCommand : ICommand
{
    public string UserId { get; set; } = string.Empty;
    public AppUserStatus Status { get; set; }
}