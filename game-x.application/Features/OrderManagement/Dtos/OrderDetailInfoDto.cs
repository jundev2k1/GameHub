namespace game_x.application.Features.OrderManagement.Dtos;

public class OrderDetailInfoDto
{
    public string? MerchantNumber { get; set; }
    public Guid OrderId { get; set; }
    public string? RedirectUrl { get; set; }
    public string UxmOrderId { get; set; } = string.Empty;
    public string OrderType { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public Guid CounterId { get; set; }
    public PricingMode PricingMode { get; set; }
    public FiatType FiatType { get; set; }
    public CryptoType CryptoType { get; set; }
    public decimal FiatAmount { get; set; }
    public decimal CryptoAmount { get; set; }
    public decimal UxmFee { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
    public int UxmOrderStatus { get; set; }
    public int UxmDisputeStatus { get; set; }
    public string Notes { get; set; } = string.Empty;
    public long UxmTimestamp { get; set; }
    public string? Metadata { get; set; }
    public string? EntryCode { get; set; }
    public string PayerBankAccountName { get; set; } = string.Empty;
    public string PayeeBankName { get; set; } = string.Empty;
    public string PayeeBranchCode { get; set; } = string.Empty;
    public string PayeeName { get; set; } = string.Empty;
    public string PayeeAccountNumber { get; set; } = string.Empty;
    public string UserPaidAt { get; set; } = string.Empty;
    public DateTime? UxmCompletedAt { get; set; }
    public DateTime? UxmCreatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}