namespace game_x.domain.Entities;

public sealed class TalentWallet : BaseEntity<string>, IAuditable
{
    public User Talent { get; private set; } = default!;
    public decimal Balance { get; private set; }

    public ICollection<TalentWalletTransaction> Transactions { get; private set; } = [];

    public static TalentWallet Create(string talentId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(talentId);

        return new TalentWallet
        {
            Id = talentId,
            Balance = 0,
        };
    }

    public void AdjustBalance(decimal newBalance) => Balance = newBalance;

    public void ClearBalance() => Balance = 0;
}
