namespace game_x.share.ExternalApi.FastPay.Dtos.ApiRequests.Deposit;

public record FastPayDepositRequest(
    string MerchantNumber,
    decimal Amount,
    string OrderNumber,
    string UserId,
    string Remark);
