namespace game_x.share.ExternalApi.Uxm.Dtos.ApiRequests.Withdrawal;

public record UxmWithdrawalRequest(
    string MerchantNumber,
    decimal Amount,
    string OrderNumber,
    string To,
    string? Remark);
