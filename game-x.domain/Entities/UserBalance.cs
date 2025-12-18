using System.ComponentModel.DataAnnotations;

namespace game_x.domain.Entities;

public sealed class UserBalance : BaseEntity<int>
{
    public Guid PublicId { get; set; }
    public string UserId { get; set; } = String.Empty;
    public User User { get; set; } = null!;
    public int CryptoTokenId { get; set; }
    public CryptoToken CryptoToken { get; set; } = null!;
    public decimal Amount { get; set; } // Available balance
    public decimal FrozenAmount { get; set; }
    public decimal TotalAmount => Amount + FrozenAmount; //  Additional support for backend queries and reporting

    [Timestamp]
    public uint? Version { get; set; }

    public static UserBalance Create(
        string userId,
        int cryptoTokenId,
        decimal amount
    )
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

    public void AdjustAmount(decimal amount, bool isIncrease)
    {
        var amountAfter = isIncrease
            ? Amount + amount
            : Amount - amount;
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amountAfter, nameof(amountAfter));

        Amount = amountAfter;
    }
}
