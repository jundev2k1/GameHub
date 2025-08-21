namespace game_x.share.ExternalApi.Uxm.Dtos;

public record UxmDepositOrderRequest(
    string MerchantNumber,
    decimal Amount,
    string OrderNumber,
    string UserId,
    string Remark);
