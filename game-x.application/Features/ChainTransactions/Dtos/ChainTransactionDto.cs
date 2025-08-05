namespace game_x.application.Features.ChainTransactions.Dtos;

public class ChainTransactionDto
{
    public Guid PublicId { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string? TransactionHash { get; set; }
    public string? FromAddress { get; set; }
    public string? ToAddress { get; set; }
    public decimal Amount { get; set; }
    public decimal Fee { get; set; }
    public int CryptoTokenId { get; set; }
    public string? CryptoTokenSymbol { get; set; }
    public DateTime ConfirmedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Note { get; set; }
    public ChainTransactionMeta? Meta { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

