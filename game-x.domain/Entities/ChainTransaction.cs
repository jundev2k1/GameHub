using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using game_x.domain.Shared;

namespace game_x.domain.Entities;

public sealed class ChainTransaction: BaseEntity<int>, IAuditable
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
    
    public static ChainTransaction Create(
        string userId,
        string orderNumber,
        decimal amount,
        int cryptoTokenId,
        ChainTransactionType type,
        ChainTransactionStatus status,
        decimal? fee = null,
        string? fromAddress = null,
        string? toAddress = null,
        string? transactionHash = null,
        string? note = null
        )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
   
        if (fee is < 0)
            throw new ArgumentException("Fee must be equal or greater than zero.", nameof(fee));
        
        var order = new ChainTransaction
        {
            UserId = userId,
            OrderNumber = orderNumber,
            TransactionHash = transactionHash,
            FromAddress = fromAddress,
            ToAddress = toAddress,
            Type = type,
            Amount = amount,
            Fee = fee ?? 0,
            CryptoTokenId = cryptoTokenId,
            Status = status,
            Note = note,
        };
        return order;
    }
    
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