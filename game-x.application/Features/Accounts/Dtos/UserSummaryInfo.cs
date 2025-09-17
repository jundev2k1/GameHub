namespace game_x.application.Features.Accounts.Dtos;

public sealed class UserSummaryInfo
{
    public string Id { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string? Avatar { get; set; }
}
