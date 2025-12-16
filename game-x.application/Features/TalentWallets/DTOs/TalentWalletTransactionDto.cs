namespace game_x.application.Features.TalentWallets.DTOs;

public sealed class TalentWalletTransactionDto
{
    public Guid Id { get; set; }
    public string TalentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public TalentTransactionType Type { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? ReferenceId { get; set; }
    public string? AdjustedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
