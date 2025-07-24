namespace game_x.application.Features.OrderManagement.Dtos;

public class OrderDto
{
    public Guid OrderId { get; set; }
    public string UxmOrderId { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string OwnerEmail { get; set; } = string.Empty;
    public Guid CounterId { get; set; }
    public string CounterNumber { get; set; } = string.Empty;
    public string StaffId { get; set; } = string.Empty;
    public string StaffName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal PricePerUnit { get; set; }
    public string CurrencyUnit { get; set; } = null!;
    public PricingMode PricingMode { get; private set; }
    public FiatType? FiatType { get; private set; }
    public CryptoType? CryptoType { get; private set; }
    public decimal FiatAmount { get; private set; }
    public decimal CryptoAmount { get; private set; }
    public decimal UxmFee { get; private set; }
    public long UxmTimestamp { get; private set; }
    public string OrderStatus { get; set; } = null!;
    public string Notes { get; set; } = string.Empty;
    public string? Metadata { get; set; }
    public string? EntryCode { get; set; }
    public string? PayeeBankName { get; set; } = string.Empty;
    public string PayeeBranchCode { get; set; } = string.Empty;
    public string? PayeeName { get; set; } = string.Empty;
    public string? PayeeAccountNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}