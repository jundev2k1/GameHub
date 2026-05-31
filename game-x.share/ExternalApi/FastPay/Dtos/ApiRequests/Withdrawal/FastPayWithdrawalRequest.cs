namespace game_x.share.ExternalApi.FastPay.Dtos.ApiRequests.Withdrawal;

public record FastPayWithdrawalRequest(
    string MerchantNumber,
    decimal Amount,
    string OrderNumber,
    string To,
    string? Remark);
