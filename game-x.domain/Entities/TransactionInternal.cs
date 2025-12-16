using System.Text.RegularExpressions;

namespace game_x.domain.Entities;

public class TransactionInternal: BaseEntity<int>, IAuditable
{
    public Transaction Transaction { get; set; } = null!;
    /// <summary>Payment Gateway ProviderId.</summary>
    public PaymentGatewayProvider ProviderId { get; set; }
    /// <summary>Provider Order Id (e.g. Uxm Service).</summary>
    public string? OrderUid { get; set; }
    /// <summary>Used to link and identify the order with other services.</summary>
    public string OrderNumber { get; set; } = string.Empty;
    /// <summary>Transaction Hash: Hash value of blockchain transaction.</summary>
    public string? Hash { get; set; }
    /// <summary>The sender's wallet address.</summary>
    public string? FromAddress { get; set; }
    /// <summary>The recipient's wallet address.</summary>
    public string? ToAddress { get; set; }
    /// <summary>The time when the transaction is completed.</summary>
    public DateTime? ConfirmedAt { get; set; }
    
    public static TransactionInternal Create(
        string orderNumber,
        PaymentGatewayProvider? providerId = null,
        string? fromAddress = null,
        string? toAddress = null)
    {
        var txInternal = new TransactionInternal
        {
            OrderNumber = orderNumber,
            FromAddress = fromAddress,
            ToAddress = toAddress,
            ProviderId = providerId ?? PaymentGatewayProvider.Uxm,
            
        };
        return txInternal;
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
}