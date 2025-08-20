namespace game_x.application.Contract.Infrastructure.SignalR.Dtos;

public class TransactionNotificationDto
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string? Hash { get; set; }
    public string? FromAddress { get; set; }
    public string? ToAddress { get; set; }
    public decimal Amount { get; set; }
    public decimal Fee { get; set; }
    public decimal? BalanceAfter { get; set; }
    public Guid CryptoTokenId { get; set; }
    public DateTime ConfirmedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string? Meta { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}