namespace game_x.domain.Entities;

public sealed class TalentWalletTransaction : BaseEntity<long>
{
    public Guid PublicId { get; private set; } = Guid.CreateVersion7();
    public string TalentId { get; private set; } = string.Empty;
    public TalentWallet TalentWallet { get; private set; } = default!;
    public decimal Amount { get; private set; }
    public TalentTransactionType Type { get; private set; }
    public decimal BalanceAfter { get; private set; }
    public string? ReferenceId { get; private set; }
    public string? AdjustedBy { get; private set; }

    public static TalentWalletTransaction Create(
        string talentId,
        TalentTransactionType type,
        decimal amount,
        decimal balanceAfter,
        string? referenceId = null,
        string? adjustedBy = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(talentId);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(balanceAfter, 0);

        return new TalentWalletTransaction
        {
            TalentId = talentId,
            Type = type,
            Amount = amount,
            BalanceAfter = balanceAfter,
            ReferenceId = referenceId,
            AdjustedBy = adjustedBy
        };
    }
}
