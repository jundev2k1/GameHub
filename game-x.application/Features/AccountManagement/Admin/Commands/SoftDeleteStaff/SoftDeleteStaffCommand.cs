namespace game_x.application.Features.AccountManagement.Admin.Commands.SoftDeleteStaff;

public sealed class SoftDeleteStaffCommand : ICommand
{
    public string UserId { get; set; } = string.Empty;
}
