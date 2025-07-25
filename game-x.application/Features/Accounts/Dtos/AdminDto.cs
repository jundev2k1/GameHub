namespace game_x.application.Features.Accounts.Dtos;

public class AdminDto
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public required bool IsNew { get; set; }
    public domain.Enum.UserStatus Status { get; set; }
    public required DateTime? CreatedAt { get; set; }
    public required DateTime? UpdatedAt { get; set; }
}