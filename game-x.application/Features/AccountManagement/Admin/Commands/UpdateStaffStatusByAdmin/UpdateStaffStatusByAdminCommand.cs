namespace game_x.application.Features.AccountManagement.Admin.Commands.UpdateStaffStatusByAdmin;

public sealed class UpdateStaffStatusByAdminCommand : ICommand
{
    public string StaffId { get; set; } = string.Empty;
    public AppUserStatus Status { get; set; }
}