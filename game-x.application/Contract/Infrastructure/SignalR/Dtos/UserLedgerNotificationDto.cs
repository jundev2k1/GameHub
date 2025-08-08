namespace game_x.application.Contract.Infrastructure.SignalR.Dtos;

public class UserUsdtLedgerNotificationDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public UsdtFlowType FlowType { get; set; }
    public string SourceId { get; set; } = string.Empty;
    public decimal ChangeAmount { get; set; }
    public decimal BalanceAfter { get; set; }
    public Guid ChainTransactionId { get; set; }
    public string Meta { get; set; } = "{}";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}