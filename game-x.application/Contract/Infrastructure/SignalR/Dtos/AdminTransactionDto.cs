namespace game_x.application.Contract.Infrastructure.SignalR.Dtos;

public class AdminTransactionDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? ActualAmount { get; set; }
    public decimal? Fee { get; set; }
    public decimal? BalanceAfter { get; set; }
    public string? Note { get; set; }
    public string? OrderNumber { get; set; }
    public string? OrderUid { get; set; }
    public Guid CryptoTokenId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Network { get; set; }  = string.Empty;
    public string Type { get; set; } =  string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
