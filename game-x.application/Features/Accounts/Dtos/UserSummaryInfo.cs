using System.Text.Json.Serialization;

namespace game_x.application.Features.Accounts.Dtos;

public sealed class UserSummaryInfo
{
    public string Id { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    [JsonIgnore]
    public int? AvatarId { get; set; }
    public string? Avatar { get; set; }
    public bool IsKycConfirmed { get; set; }
    public bool IsBankAccountConfirmed { get; set; }
}
