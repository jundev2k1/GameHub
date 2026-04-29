namespace game_x.share.ExternalApi.FastPay.Dtos.Webhooks.DepositSuccess;

public record DepositSucessCallbackRequest(
    string MerchantNumber,
    decimal Amount,
    string OrderNumber,
    string To,
    string? Remark);
