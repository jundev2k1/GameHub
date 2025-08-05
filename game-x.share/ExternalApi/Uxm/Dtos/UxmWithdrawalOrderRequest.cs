namespace game_x.share.ExternalApi.Uxm.Dtos;

public record UxmWithdrawalOrderRequest(
    string MerchantNumber,
    decimal Amount,
    string OtcOrderNumber,
    string To,
    string? Remark);