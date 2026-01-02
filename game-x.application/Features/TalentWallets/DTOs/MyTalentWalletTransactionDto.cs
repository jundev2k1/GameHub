namespace game_x.application.Features.TalentWallets.DTOs;

public sealed class MyTalentWalletTransactionDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public TalentTransactionType Type { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? AdjustedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}
