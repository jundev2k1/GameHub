using game_x.domain.Exceptions;
using game_x.domain.Shared;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace game_x.domain.Entities;

public class Transaction : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; private set; } = Guid.CreateVersion7();
    public string UserId { get; private set; } = string.Empty;
    public User User { get; private set; } = null!;
    /// <summary>The funds used for the transaction.</summary>
    public decimal Amount { get; private set; }
    /// <summary>The amount of funds the user actually transferred.</summary>
    public decimal? ActualAmount { get; private set; }
    public decimal? Fee { get; private set; }
    public int CryptoTokenId { get; private set; }
    public CryptoToken CryptoToken { get; private set; } = null!;
    public TransactionSourceType SourceType { get; private set; }
    public TransactionType Type { get; private set; }
    public TransactionStatus Status { get; private set; }
    public decimal? BalanceAfter { get; private set; }
    public decimal GameAmount { get; private set; }
    public decimal? GameBalanceAfter { get; private set; }
    public string Meta { get; private set; } = "{}";
    public string? Note { get; private set; }
    public TransactionInternal? TransactionInternal { get; private set; }
    public TransactionExternal? TransactionExternal { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? ReviewedById { get; private set; }
    public User? ReviewedBy { get; private set; }
    public DateTime? DateReviewed { get; private set; }

    #region Handle Timeout transaction

    public DateTime? ExpiredAt { get; private set; }
    /// <summary>
    /// The transaction used to refund money to the user in the case where the user deposits money into a transaction that has timed out.
    /// </summary>
    public int? RefundTransactionId { get; private set; }

    #endregion

    public decimal TotalAmount => Amount + (Fee ?? 0);

    public static Transaction Create(
        string userId,
        decimal amount,
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
        TransactionInternal = txInternal;
    }

    public void AddTxExternal(TransactionExternal txExternal)
    {
        TransactionExternal = txExternal;
    }

    public void UpdateProviderResponse(
        decimal? balanceAfter,
        decimal? amount = null,
        decimal? actualAmount = null,
        string? providerOrderId = null,
        string? hash = null,
        string? to = null,
        DateTime? confirmedAt = null,
        DateTime? completedAt = null)
    {
        BalanceAfter = balanceAfter;
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

    public void Review(bool isApprove, string reviewedById)
    {
        Status = isApprove
            ? TransactionStatus.Approved
            : TransactionStatus.Rejected;
        ReviewedById = reviewedById;
        DateReviewed = DateTime.UtcNow;
    }

    public void Confirm(decimal actualAmount, decimal balanceAfter)
    {
        ActualAmount = actualAmount;
        BalanceAfter = balanceAfter;
        Status = TransactionStatus.Completed;
        GameBalanceAfter = null;

        if (TransactionInternal != null)
            TransactionInternal.ConfirmedAt = DateTime.UtcNow;
    }

    public void ConfirmGameTx(decimal actualAmount, decimal balanceAfter, decimal gameBalanceAfter)
    {
        ActualAmount = actualAmount;
        BalanceAfter = balanceAfter;
        GameBalanceAfter = gameBalanceAfter;
        Status = TransactionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (!CanCancelTransaction())
            throw new BusinessRuleViolationException($"Current Status ({Status}) cannot cancel.");

        Status = TransactionStatus.Failed;
    }

    public bool CanCancelTransaction()
        => Status is (TransactionStatus.Pending or TransactionStatus.Approved);
    
    public void Expired()
    {
        ExpiredAt = DateTime.UtcNow;
    }
    
    public void Refund(int transactionId)
    {
        RefundTransactionId = transactionId;
    }
}

public class TransactionMeta
{
    public string ErrorMessage { get; set; } = string.Empty;
}