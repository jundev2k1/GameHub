using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using game_x.domain.Shared;

namespace game_x.domain.Entities;

public sealed class ChainTransaction : BaseEntity<int>
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
    public ChainTransactionStatus Status { get; set; } = ChainTransactionStatus.Pending;
    public string Meta { get; set; } = "{}";
    public string? Note { get; set; }

    public static ChainTransaction Create(
    string orderNumber,
    decimal amount,
    decimal fee,
    int cryptoTokenId,
    string? userId = null,
    string? transactionHash = null,
    string? fromAddress = null,
    string? toAddress = null,
    string? note = null)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new ArgumentException("OrderNumber cannot be null or empty.", nameof(orderNumber));

        return new ChainTransaction
        {
            PublicId = Guid.NewGuid(),
            UserId = userId,
            OrderNumber = orderNumber,
            TransactionHash = transactionHash,
            FromAddress = fromAddress,
            ToAddress = toAddress,
            Amount = amount,
            Fee = fee,
            CryptoTokenId = cryptoTokenId,
            ConfirmedAt = DateTime.UtcNow,
            Status = ChainTransactionStatus.Pending,
            Meta = JsonSerializer.Serialize(new ChainTransactionMeta(), JsonOptions.NoEscape),
            Note = note
        };
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
