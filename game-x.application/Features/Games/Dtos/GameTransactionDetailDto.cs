using GamePlatform = game_x.domain.Entities.GamePlatform;

namespace game_x.application.Features.Games.Dtos;

public class GameTransactionDetailDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } =  string.Empty;
    public string? G598Sno { get; set; }
    public decimal Amount { get; set; }
    public decimal? BalanceAfter { get; set; }
    public Guid CryptoTokenId { get; set; }
    public string? Note { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public int GamePlatformId { get; set; }
    public GamePlatform GamePlatform { get; set; } = default!;
    public NetworkType Network { get; set; }
    public GameTransactionType Type { get; set; }
    public GameTransactionStatus Status { get; set; }
    public string Meta { get; set; } = "{}";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}