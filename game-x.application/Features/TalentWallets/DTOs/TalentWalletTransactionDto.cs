namespace game_x.application.Features.TalentWallets.DTOs;

public sealed class TalentWalletTransactionDto
{
    public Guid Id { get; set; }
    public string TalentId { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public TalentTransactionType Type { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? DonorEmail { get; set; }
    public string? DonorNickname { get; set; }
    public decimal? DonatedAmount { get; set; }
    public decimal? TalentReceive { get; set; }
    public decimal? SystemReceive { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? AdjustedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
