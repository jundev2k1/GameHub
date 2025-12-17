namespace game_x.application.Features.UserGameSessions.Dtos;

public sealed class UserGameSessionSearchItemDto
{
    public string UserId { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public Guid PlatformId { get; set; }
    public string? GameCode { get; set; }
    public decimal BalanceSnapshot { get; set; }
    public DateTime ConnectedAt { get; set; }
    public DateTime? DisconnectedAt { get; set; }
}
