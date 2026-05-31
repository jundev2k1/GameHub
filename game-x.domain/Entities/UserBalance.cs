using game_x.domain.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace game_x.domain.Entities;

public sealed class UserBalance : BaseEntity<int>
{
    public Guid PublicId { get; private set; }
    public string UserId { get; private set; } = string.Empty;
    public User User { get; private set; } = null!;
    public int CryptoTokenId { get; private set; }
    public CryptoToken CryptoToken { get; private set; } = null!;
    public decimal Amount { get; private set; } // Available balance
    public decimal FrozenAmount { get; private set; }
    public decimal TotalAmount => Amount + FrozenAmount; //  Additional support for backend queries and reporting

    [Timestamp]
    public uint? Version { get; set; }

    public static UserBalance Create(
        string userId,
        int cryptoTokenId,
        decimal amount)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        var userBalance = new UserBalance
        {
            UserId = userId,
            Amount = amount,
            CryptoTokenId = cryptoTokenId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        return userBalance;
    }

    public static decimal GetWithdrawalFee() => 0m;

    public void AdjustAmount(decimal amount, bool isIncrease)
    {
        var amountAfter = isIncrease
            ? Amount + amount
            : Amount - amount;
        ArgumentOutOfRangeException.ThrowIfLessThan(amountAfter, 0);

        Amount = amountAfter;
    }

    public void Freeze(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount, nameof(amount));

        if (Amount < amount)
            throw new InsufficientBalanceException(Amount, amount);

        Amount -= amount;
        FrozenAmount += amount;
    }

    public void Unfreeze(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount, nameof(amount));

        if (FrozenAmount < amount)
            throw new InsufficientBalanceException(FrozenAmount, amount);

        FrozenAmount -= amount;
        Amount += amount;
    }

    public void FinalizeFrozen(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount, nameof(amount));

        if (FrozenAmount < amount)
            throw new InsufficientBalanceException(FrozenAmount, amount);

        FrozenAmount -= amount;
    }
}