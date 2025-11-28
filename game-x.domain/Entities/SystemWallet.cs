namespace game_x.domain.Entities;

public sealed class SystemWallet : BaseEntity<Guid>, IAuditable
{
    public SystemWalletType Type { get; private set; }
    public decimal Balance { get; private set; }

    public ICollection<SystemWalletTransaction> Transactions { get; private set; } = [];

    public static SystemWallet Create(SystemWalletType type)
    {
        return new SystemWallet
        {
            Id = Guid.CreateVersion7(),
            Type = type,
            Balance = 0,
        };
    }

    public void AdjustBalance(decimal balance) => Balance = balance;
}
