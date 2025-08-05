using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using game_x.domain.Shared;

namespace game_x.domain.Entities;

public sealed class UserUsdtLedger: BaseEntity<int>
{
    public Guid PublicId { get; set; }
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;

    public DateTime Timestamp { get; set; }      // Time
    public UsdtFlowType FlowType { get; set; }   // Deposit, Withdrawal, etc.
    public string SourceId { get; set; } = string.Empty; // Raw Record ID
    public decimal ChangeAmount { get; set; }    // Current Transaction Amount Change

    public decimal BalanceAfter { get; set; }    // balance after change
    
    // Associated raw data
    public int? ChainTransactionId { get; set; }
    public ChainTransaction? ChainTransaction { get; set; }

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