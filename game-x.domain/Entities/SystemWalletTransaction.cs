namespace game_x.domain.Entities;

public sealed class SystemWalletTransaction : BaseEntity<int>
{
    public Guid WalletId { get; private set; }
    public SystemWallet Wallet { get; private set; } = default!;
    public decimal Amount { get; private set; }
    public decimal? BalanceAfter { get; private set; }
    public string? ReferenceId { get; private set; }

    public static SystemWalletTransaction Create(
        Guid walletId,
        decimal amount,
        decimal? balanceAfter,
        string? referenceId = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        return new SystemWalletTransaction
        {
            WalletId = walletId,
            Amount = amount,
            BalanceAfter = balanceAfter,
            ReferenceId = referenceId,
        };
    }
}
