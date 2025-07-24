namespace game_x.share.ExternalApi.Uxm.Dtos;

public record CreateSellOrderV2Request(
    string MerchantNumber,
    string MerchantOrderId,
    string MemberId,
    decimal Amount,
    int PricingMode,
    int FiatType,
    int CryptoType,
    long Timestamp,
    string PayeeBankName,
    string PayeeBranchCode,
    string PayeeName,
    string PayeeAccountNumber);
