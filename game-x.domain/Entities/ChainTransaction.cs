using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using game_x.domain.Shared;

namespace game_x.domain.Entities;

public sealed class ChainTransaction: BaseEntity<int>
{
    public Guid PublicId { get; set; }
    public string? UserId { get; set; }
    public User? User { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string? TransactionHash { get; set; }
    public string? FromAddress { get; set; }
    public string? ToAddress { get; set; }
    public decimal Amount { get; set; }
    public decimal Fee { get; set; }
    public int CryptoTokenId { get; set; }
    public CryptoToken CryptoToken { get; set; } = null!;
    public DateTime ConfirmedAt { get; set; } = DateTime.UtcNow;
    public ChainTransactionType Type { get; set; }
    public ChainTransactionStatus Status { get; set; } = ChainTransactionStatus.Pending;
    public string Meta { get; set; } = "{}";
    public string? Note { get; set; }
    
    [NotMapped]
    public ChainTransactionMeta MetaObject
    {
        get => JsonSerializer.Deserialize<ChainTransactionMeta>(Meta) ?? new();
        set => Meta = JsonSerializer.Serialize(value, JsonOptions.NoEscape);
    }
    
    public void UpdateMeta(Action<ChainTransactionMeta> updater)
    {
        var meta = MetaObject;
        updater(meta);
        MetaObject = meta;
    }
}

public class ChainTransactionMeta
{
    public string ErrorMessage { get; set; } = string.Empty;
}