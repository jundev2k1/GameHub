using game_x.domain.Identity;

namespace game_x.application.Features.AccountManagement.Dtos;

public class UserMappingDto : AppUser
{
    public AppUser? Staff { get; set; }
}

public class UserDto
{
    public required string Id { get; set; }
    public required string PhoneNumber { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public bool IsNew { get; set; }
    public AppUserStatus? Status { get; set; }
    public string? PassportNumber { get; set; }
    public string? CountryCode { get; set; }
    public required string CreatedById { get; set; }
    public required string CreatedByName { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}