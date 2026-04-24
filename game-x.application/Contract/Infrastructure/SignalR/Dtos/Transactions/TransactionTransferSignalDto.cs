namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;

public sealed record TransactionTransferSignalDto
{
    public Guid Id { get; set; }
    public string? ReceiverId { get; set; }
    public string? TransferorId { get; set; }
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? BalanceAfter { get; set; }
    public string? Note { get; set; }
    public Guid CryptoTokenId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string? Network { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public string? SourceType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}