namespace game_x.share.ExternalApi.Uxm.Dtos;

public record GetUxmOrderDetailInfoRequest(
    string MerchantNumber,
    string TradeNo,
    long Timestamp);