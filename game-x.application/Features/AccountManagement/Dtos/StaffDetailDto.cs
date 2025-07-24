namespace game_x.application.Features.AccountManagement.Dtos;

public class StaffDetailDto
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public required bool IsNew { get; set; }
    public AppUserStatus Status { get; set; }
    public required string[] Roles { get; set; } = [];
    public required string CreatedById { get; set; }
    public required string CreatedByName { get; set; }
    public required DateTime? CreatedAt { get; set; }
    public required DateTime? UpdatedAt { get; set; }
}