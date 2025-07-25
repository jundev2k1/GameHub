namespace game_x.application.Features.Accounts.Dtos;

public class UserDetailDto
{
    public required string Id { get; set; }
    public required string PhoneNumber { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required bool IsNew { get; set; }
    public domain.Enum.UserStatus Status { get; set; }
    public string? CountryCode { get; set; }
    public required string[] Roles { get; set; } = [];
    public required string CreatedById { get; set; }
    public required string CreatedByName { get; set; }
}