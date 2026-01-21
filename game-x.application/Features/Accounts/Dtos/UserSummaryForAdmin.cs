using System.Text.Json.Serialization;

namespace game_x.application.Features.Accounts.Dtos;

public sealed class UserSummaryForAdmin
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    [JsonIgnore]
    public UserStatus Status { get; set; }
    [JsonIgnore]
    public int? AvatarId { get; set; }
    public string? Avatar { get; set; }
    public string[] Roles { get; set; } = [];
    public bool IsEmailConfirmed { get; set; }
    public bool IsKycConfirmed { get; set; }
    public bool IsBankAccountConfirmed { get; set; }
    public bool IsActive { get; set; }
}
