namespace game_x.share.ExternalApi.Uxm.Dtos;

public record UxmDepositOrderRequestData(
    string MerchantNumber,
    decimal Amount,
    string OrderNumber,
    string UserId,
    string Remark);
