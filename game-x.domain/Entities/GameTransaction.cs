using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using game_x.domain.Shared;

namespace game_x.domain.Entities;

public sealed class GameTransaction : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public string G598Sno { get; private set; } = string.Empty;
    public string UserId { get; private set; } = string.Empty;
    public User User { get; private set; } = null!;
    public GameTransactionType Type { get; private set; }
    public GameTransactionStatus Status { get; private set; } = GameTransactionStatus.Pending;
    public decimal Amount { get; private set; }
    public int GamePlatformId { get; private set; } = default!;
    public GamePlatform GamePlatform { get; private set; } = default!;
    public string? Note { get; private set; } = string.Empty;
    public string Meta { get; private set; } = "{}";
    public int? CryptoTokenId { get; private set; }
    public CryptoToken? CryptoToken { get; private set; }
    public UserUsdtLedger? Ledger { get; private set; }
    
    [NotMapped]
    public ChainTransactionMeta MetaObject
    {
        get => JsonSerializer.Deserialize<ChainTransactionMeta>(Meta) ?? new();
        set => Meta = JsonSerializer.Serialize(value, JsonOptions.NoEscape);
    }
    
    public static GameTransaction Create(
        string userId,
        string g598sno,
        decimal amount,
        GameTransactionType type,
        int gamePlatformId,
        int? cryptoTokenId,
        string? note = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        return new GameTransaction
        {
            UserId = userId,
            G598Sno = g598sno,
            Amount = amount,
            Status = GameTransactionStatus.Pending,
            Type = type,
            CryptoTokenId = cryptoTokenId,
            GamePlatformId = gamePlatformId,
            Note = note
        };
    }

    public void UpdateStatus(GameTransactionStatus status)
    {
        Status = status;
    }

    public void UpdateMeta(Action<ChainTransactionMeta> updater)
    {
        var meta = MetaObject;
        updater(meta);
        MetaObject = meta;
    }
}
