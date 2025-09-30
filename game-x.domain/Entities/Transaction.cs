using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using game_x.domain.Shared;

namespace game_x.domain.Entities;

public class Transaction: BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;
    /// <summary>The funds used for the transaction.</summary>
    public decimal Amount { get; set; }
    /// <summary>The amount of funds the user actually transferred.</summary>
    public decimal? ActualAmount { get; set; }
    public decimal? Fee { get; set; }
    public int CryptoTokenId { get; set; }
    public CryptoToken CryptoToken { get; set; } = null!;
    public TransactionSourceType SourceType { get; set; }
    public TransactionType Type { get; set; }
    public TransactionStatus Status { get; set; }
    public decimal? BalanceAfter { get; set; }
    public string Meta { get; set; } = "{}";
    public string? Note { get; set; }
    public TransactionInternal? TransactionInternal { get; set; }
    public TransactionExternal? TransactionExternal { get; set; }
    
    public decimal TotalAmount => Amount + (Fee ?? 0);

    public static Transaction Create(
        string userId,
        decimal amount,
        int cryptoTokenId,
        TransactionSourceType sourceType,
        TransactionType type,
        TransactionStatus? status = null,
        decimal? fee = null,
        string? note = null
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

        if (fee is < 0)
            throw new ArgumentException("Fee must be equal or greater than zero.", nameof(fee));

        var chainTransaction = new Transaction
        {
            UserId = userId,
            SourceType = sourceType,
            Type = type,
            Amount = amount,
            Fee = fee,
            CryptoTokenId = cryptoTokenId,
            Status = status ?? TransactionStatus.Pending,
            Note = note,
        };
        return chainTransaction;
    }
    
    [NotMapped]
    public TransactionMeta MetaObject
    {
        get => JsonSerializer.Deserialize<TransactionMeta>(Meta) ?? new();
        set => Meta = JsonSerializer.Serialize(value, JsonOptions.NoEscape);
    }
    
    public void UpdateMeta(Action<TransactionMeta> updater)
    {
        var meta = MetaObject;
        updater(meta);
        MetaObject = meta;
    }
    
    public void UpdateStatus(TransactionStatus status)
    {
        Status = status;
    }
    
    public void AddTxInternal(TransactionInternal txInternal)
    {
        TransactionInternal =  txInternal;
    }
    
    public void AddTxExternal(TransactionExternal txExternal)
    {
        TransactionExternal =  txExternal;
    }
    
    public void UpdateUxmResponse(
        decimal? amount = null,
        decimal? actualAmount = null,
        string? orderUid = null, 
        string? hash = null,
        string? to = null,
        DateTime? confirmedAt = null)
    {
        Amount = amount ?? Amount;
        ActualAmount = actualAmount ?? ActualAmount;
        if (TransactionInternal != null)
        {
            TransactionInternal.OrderUid = orderUid ?? TransactionInternal.OrderUid;
            TransactionInternal.Hash = hash ?? TransactionInternal.Hash;
            TransactionInternal.ToAddress = to ?? TransactionInternal.ToAddress;
            TransactionInternal.ConfirmedAt = confirmedAt ?? TransactionInternal.ConfirmedAt;
        }
    }

    public void ConfirmTx(decimal actualAmount, DateTime confirmedAt)
    {
        ActualAmount = actualAmount;
        Status = TransactionStatus.Completed;
        if (TransactionInternal != null)
        {
            TransactionInternal.ConfirmedAt = confirmedAt;
        }
    }
}

public class TransactionMeta
{
    public string ErrorMessage { get; set; } = string.Empty;
}