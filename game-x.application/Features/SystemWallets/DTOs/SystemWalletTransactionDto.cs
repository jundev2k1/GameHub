namespace game_x.application.Features.SystemWallets.DTOs;

public sealed class SystemWalletTransactionDto
{
    public Guid WalletId { get; set; }
    public SystemWalletType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal? BalanceAfter { get; set; }
    public string? ReferenceId { get; set; }
    public DateTime CreatedAt { get; set; }
}
