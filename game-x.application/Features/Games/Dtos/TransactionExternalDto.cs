namespace game_x.application.Features.Games.Dtos;

public class TransactionExternalDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } =  string.Empty;
    public string Email { get; set; } =  string.Empty;
    public string Nickname { get; set; } =  string.Empty;
    public decimal Amount { get; set; }
    public decimal? BalanceAfter { get; set; }
    public Guid CryptoTokenId { get; set; }
    public string? Note { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public Guid GamePlatformId { get; set; }
    public string GamePlatformName { get; set; } = string.Empty;
    public NetworkType Network { get; set; }
    public TransactionType Type { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}