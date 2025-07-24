namespace game_x.application.Features.Accounts.Dtos;

public class UserDto
{
    public string Id { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsNew { get; set; }
    public AppUserStatus? Status { get; set; }
    public string? PassportNumber { get; set; }
    public string? CountryCode { get; set; }
    public string CreatedById { get; set; } = string.Empty;
    public string CreatedByName { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
