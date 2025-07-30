namespace game_x.share.ExternalApi.Uxm.Dtos;

public record CreateOrderBuyRequestData(
    string MerchantNumber,
    decimal Amount,
    string OrderNumber,
    string UserId,
    string Remark);
