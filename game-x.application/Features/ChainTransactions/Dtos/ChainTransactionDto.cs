namespace game_x.application.Features.ChainTransactions.Dtos;

public class ChainTransactionDto
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public decimal Amount { get; set; }
    public decimal Fee { get; set; }
    public Guid CryptoTokenId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public NetworkType Network { get; set; }
    public ChainTransactionType Type { get; set; }
    public ChainTransactionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

