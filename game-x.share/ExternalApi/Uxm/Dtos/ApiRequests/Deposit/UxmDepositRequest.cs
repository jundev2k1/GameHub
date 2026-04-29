namespace game_x.share.ExternalApi.Uxm.Dtos.ApiRequests.Deposit;

public record UxmDepositRequest(
    string MerchantNumber,
    decimal Amount,
    string OrderNumber,
    string UserId,
    string Remark);
