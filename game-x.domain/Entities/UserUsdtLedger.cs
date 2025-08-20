using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using game_x.domain.Shared;

namespace game_x.domain.Entities;

public sealed class UserUsdtLedger : BaseEntity<int>
{
    public Guid PublicId { get; set; }
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;

    public DateTime Timestamp { get; set; }
    /// <summary>The transaction type</summary>
    public UsdtFlowType FlowType { get; set; }
    /// <summary>Raw Record ID</summary>
    public string SourceId { get; set; } = string.Empty;
    /// <summary>Current Transaction Amount Change</summary>
    public decimal ChangeAmount { get; set; }
    /// <summary>The user balance after change</summary>
    public decimal BalanceAfter { get; set; }
    /// <summary>Status at the time the event takes place</summary>
    public string? StatusAtEvent { get; set; }

    /// <summary>The chain transaction </summary>
    public int? ChainTransactionId { get; set; }
    public ChainTransaction? ChainTransaction { get; set; }

    // NEW: Default = UXM để backward compatibility
    public LedgerType Type { get; set; } = LedgerType.Uxm;

    // NEW: Game transaction field - nullable
    public int? GameTransactionId { get; set; }
    public GameTransaction? GameTransaction { get; set; }

    public string Meta { get; set; } = "{}";

    [NotMapped]
    public UserUsdtLedgerMeta MetaObject
    {
        get => JsonSerializer.Deserialize<UserUsdtLedgerMeta>(Meta) ?? new();
        set => Meta = JsonSerializer.Serialize(value, JsonOptions.NoEscape);
    }

    public void UpdateMeta(Action<UserUsdtLedgerMeta> updater)
    {
        var meta = MetaObject;
        updater(meta);
        MetaObject = meta;
    }
}

public class UserUsdtLedgerMeta
{
    public string? CounterpartyUserId { get; set; } // Counterparty ID
    public string? CounterpartyName { get; set; }   // Counterparty Name
}