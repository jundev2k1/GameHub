namespace game_x.share.ExternalApi.Uxm.Dtos;

public class CreateBuyOrderV2ReqData
{
    public required string MerchantNumber { get; set; }
    public required string MerchantOrderId { get; set; }
    public required string MemberId { get; set; }
    public required string PayerBankAccountName { get; set; }
    public required int PricingMode { get; set; }
    public required decimal Amount { get; set; }
    public required int FiatType { get; set; }
    public required int CryptoType { get; set; }
    public required long Timestamp { get; set; }
}