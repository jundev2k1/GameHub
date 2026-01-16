using System.Text.RegularExpressions;

namespace game_x.domain.Entities;

public class TransactionInternal : BaseEntity<int>
{
    public Transaction Transaction { get; private set; } = null!;
    /// <summary>Payment Gateway ProviderId.</summary>
    public PaymentGatewayProvider? ProviderId { get; private set; }
    public TransactionSourceType SourceType { get; private set; } = TransactionSourceType.Uxm;

    #region Transfer with the UXM service
    /// <summary>Provider Order Id (e.g. Uxm Service).</summary>
    public string? OrderUid { get; private set; }
    /// <summary>Used to link and identify the order with other services.</summary>
    public string? OrderNumber { get; private set; }
    /// <summary>Transaction Hash: Hash value of blockchain transaction.</summary>
    public string? Hash { get; private set; }
    /// <summary>The sender's wallet address.</summary>
    public string? FromAddress { get; private set; }
    /// <summary>The recipient's wallet address.</summary>
    public string? ToAddress { get; private set; }
    /// <summary>The time when the transaction is completed.</summary>
    public DateTime? ConfirmedAt { get; private set; }
    #endregion

    #region Transfer between friends
    public string? ReceiverId { get; private set; }
    public User? Receiver { get; private set; }
    public string? TransferorId { get; private set; }
    public User? Transferor { get; private set; }
    #endregion

    public static TransactionInternal Create(
        string? orderNumber = null,
        string? referenceId = null,
        PaymentGatewayProvider? providerId = null,
        string? fromAddress = null,
        string? toAddress = null,
        TransactionSourceType? sourceType = null,
        string? transferorId = null,
        string? receiverId = null)
    {
        return new()
        {
            OrderNumber = orderNumber,
            OrderUid = referenceId,
            FromAddress = fromAddress,
            ToAddress = toAddress,
            ProviderId = providerId,
            SourceType = sourceType ?? TransactionSourceType.Uxm,
            TransferorId = transferorId,
            ReceiverId = receiverId,
        };
    }

    public static bool IsValidAddress(NetworkType network, string address)
    {
        var trc20Regexp = @"^T[a-zA-Z0-9]{33}$";
        var erc20Regexp = @"^0x[a-fA-F0-9]{40}$";

        if (address.IsNullOrWhiteSpace())
            return false;

        switch (network)
        {
            case NetworkType.Tron:
                return Regex.IsMatch(address, trc20Regexp, RegexOptions.IgnoreCase);
            case NetworkType.Ethereum:
                return Regex.IsMatch(address, erc20Regexp, RegexOptions.IgnoreCase);
            default:
                return false;
        }
    }

    public void Confirm() {
        ConfirmedAt = DateTime.UtcNow;
    }
    
    public void UpdateUxmInfo(
        string? providerOrderId = null,
        string? hash = null,
        string? to = null,
        DateTime? confirmedAt = null)
    {
        OrderUid = providerOrderId ?? OrderUid;
        Hash = hash ?? Hash;
        ToAddress = to ?? ToAddress;
        ConfirmedAt = confirmedAt ?? ConfirmedAt;
    }
}