using System.Text.Json.Serialization;

namespace game_x.application.Features.Accounts.Dtos;

public class UserDetailDto
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string? FullName { get; set; } = string.Empty;
    [JsonIgnore]
    public int? AvatarId { get; set; }
    public string? AvatarUrl { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? ResidentialAddress { get; set; } = string.Empty;
    public bool IsEmailConfirmed { get; set; }
    public bool IsKycConfirmed { get; set; }
    public bool IsBankConfirmed { get; set; }
    public BalanceInfo[] Balances { get; set; } = [];
    public UserExtendDto? UserExtendInfo { get; set; } = default!;
    public AppRole Roles { get; set; } = default!;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public sealed class BalanceInfo
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public Guid CryptoTokenId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public NetworkType Network { get; set; }
    public decimal Amount { get; set; }
    public decimal FrozenAmount { get; set; }
    public decimal TotalAmount { get; set; }
}
