namespace game_x.domain.Entities;

public class TransactionInternal: BaseEntity<int>, IAuditable
{
    public int TransactionId { get; set; }
    public Transaction Transaction { get; set; } = null!;
    /// <summary>UXM's Order ID: The Order ID is returned from the UXM service.</summary>
    public string? OrderUid { get; set; }
    /// <summary>Used to link and identify the order with other services.</summary>
    public string OrderNumber { get; set; } = string.Empty;
    /// <summary>Transaction Hash: Hash value of blockchain transaction.</summary>
    public string? Hash { get; set; }
    /// <summary>The sender's wallet address.</summary>
    public string? FromAddress { get; set; }
    /// <summary>The recipient's wallet address.</summary>
    public string? ToAddress { get; set; }
    /// <summary>The funds used for the transaction.</summary>
    public DateTime? ConfirmedAt { get; set; }
    
    public static TransactionInternal Create(
        string orderNumber,
        string? fromAddress = null,
        string? toAddress = null
    )
    {
        var tx = new TransactionInternal
        {
            OrderNumber = orderNumber,
            FromAddress = fromAddress,
            ToAddress = toAddress,
        };
        return tx;
    }
    
    public void UpdateUxmResponse(string orderUid, string hash, DateTime? confirmedAt)
    {
        OrderUid = orderUid;
        Hash = hash;
        ConfirmedAt = confirmedAt ?? DateTime.UtcNow;
    }
}