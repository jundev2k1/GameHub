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
    public decimal GameAmount { get; set; }
    public decimal? GameBalanceAfter { get; set; }
    public string Meta { get; set; } = "{}";
    public string? Note { get; set; }
    public TransactionInternal? TransactionInternal { get; set; }
    public TransactionExternal? TransactionExternal { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    public decimal TotalAmount => Amount + (Fee ?? 0);

    public static Transaction Create(
        string userId,
        decimal amount,
        decimal gameAmount,
        int cryptoTokenId,
        TransactionSourceType sourceType,
        TransactionType type,
        TransactionStatus? status = null,
        decimal? fee = null,
        string? note = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));

        if (fee is < 0)
            throw new ArgumentException("Fee must be equal or greater than zero.", nameof(fee));

        return new()
        {
            UserId = userId,
            SourceType = sourceType,
            Type = type,
            Amount = amount,
            GameAmount = gameAmount,
            Fee = fee,
            CryptoTokenId = cryptoTokenId,
            Status = status ?? TransactionStatus.Pending,
            Note = note,
        };
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
    
    public void UpdateProviderResponse(
        decimal? amount = null,
        decimal? actualAmount = null,
        string? providerOrderId = null, 
        string? hash = null,
        string? to = null,
        DateTime? confirmedAt = null,
        DateTime? completedAt = null)
    {
        Amount = amount ?? Amount;
        ActualAmount = actualAmount ?? ActualAmount;
        CompletedAt = completedAt ?? CompletedAt;
        if (TransactionInternal != null)
        {
            TransactionInternal.OrderUid = providerOrderId ?? TransactionInternal.OrderUid;
            TransactionInternal.Hash = hash ?? TransactionInternal.Hash;
            TransactionInternal.ToAddress = to ?? TransactionInternal.ToAddress;
            TransactionInternal.ConfirmedAt = confirmedAt ?? TransactionInternal.ConfirmedAt;
        }
    }

    public void Confirm(decimal actualAmount, decimal balanceAfter)
    {
        ActualAmount = actualAmount;
        BalanceAfter = balanceAfter;
        Status = TransactionStatus.Completed;
        GameAmount = 0;
        GameBalanceAfter = null;

        if (TransactionInternal != null)
            TransactionInternal.ConfirmedAt = DateTime.UtcNow;
    }

    public void ConfirmGameTx(decimal balanceAfter, decimal gameBalanceAfter)
    {
        ActualAmount = balanceAfter;
        BalanceAfter = balanceAfter;
        GameBalanceAfter = gameBalanceAfter;
    }
}

public class TransactionMeta
{
    public string ErrorMessage { get; set; } = string.Empty;
}