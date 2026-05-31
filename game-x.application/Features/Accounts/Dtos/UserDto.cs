
namespace game_x.application.Features.Accounts.Dtos;

public class UserDto
{
    public required string Id { get; set; }
    public required string Nickname { get; set; }
    public required string MemberNumber { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public UserStatus? Status { get; set; }
    public string? CountryCode { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}