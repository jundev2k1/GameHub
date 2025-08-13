namespace game_x.domain.Entities;

public sealed class GameTransaction : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();

    /// <summary>Unique transaction number sent to game platform (max 30 chars).</summary>
    public string G598Sno { get; private set; } = string.Empty;

    public string UserId { get; private set; } = string.Empty;
    public User User { get; private set; } = default!;

    /// <summary>Deposit or Withdrawal.</summary>
    public GameTransactionType Type { get; private set; }

    /// <summary>Amount transferred.</summary>
    public decimal Amount { get; private set; }

    /// <summary>Game platform.</summary>
    public GamePlatform GamePlatform { get; private set; }

    public string? Note { get; private set; } = string.Empty;

    public static GameTransaction Create(
        string userId,
        string g598sno,
        decimal amount,
        GamePlatform gamePlatform,
        GameTransactionType type,
        string? note = null
    )
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException(nameof(userId));

        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        return new GameTransaction
        {
            UserId = userId,
            G598Sno = g598sno,
            Amount = amount,
            GamePlatform = gamePlatform,
            Type = type,
            Note = note
        };
    }

}
