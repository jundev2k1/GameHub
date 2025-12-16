namespace game_x.application.Features.ChainTransactions.Dtos;

public class TransactionInternalDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? ActualAmount { get; set; }
    public decimal? Fee { get; set; }
    public decimal? BalanceAfter { get; set; }
    public string? Note { get; set; }
    public Guid CryptoTokenId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public NetworkType Network { get; set; }
    public TransactionType Type { get; set; }
    public TransactionStatus Status { get; set; }
    public string? OrderUid { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string? Hash { get; set; }
    public string? FromAddress { get; set; }
    public string? ToAddress { get; set; }
    public DateTime? ConfirmedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}