namespace game_x.domain.Entities;

public sealed class GameTransaction : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; set; }

    /// <summary>Unique transaction number sent to game platform (max 30 chars).</summary>
    public string Sno { get; set; } = string.Empty;

    public string? UserId { get; set; }
    public User? User { get; set; }

    /// <summary>Deposit or Withdrawal.</summary>
    public GameTransactionType Type { get; set; }

    public GameTransactionStatus Status { get; set; } = GameTransactionStatus.Pending;

    /// <summary>Amount transferred.</summary>
    public decimal Amount { get; set; }

    /// <summary>Game platform ID or name.</summary>
    public string GamePlatform { get; set; } = string.Empty;

    public string? Note { get; set; }

    public static GameTransaction Create(
        string userId,
        string sno,
        decimal amount,
        string gamePlatform,
        GameTransactionType type,
        GameTransactionStatus status,
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
            Sno = sno,
            Amount = amount,
            GamePlatform = gamePlatform,
            Type = type,
            Status = status,
            Note = note
        };
    }

    public void UpdateStatus(GameTransactionStatus status) => Status = status;
}
