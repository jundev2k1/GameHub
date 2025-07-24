namespace game_x.share.ExternalApi.Uxm.Dtos;

public record EstimateQuoteRequest(
    string MerchantNumber,
    int Direction,
    decimal Amount,
    int PricingMode,
    int FiatType,
    int CryptoType,
    long Timestamp);