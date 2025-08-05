namespace game_x.share.ExternalApi.Uxm.Dtos;

public record CreateChainTransactionDepositRequestData(
    string MerchantNumber,
    decimal Amount,
    string OrderNumber,
    string UserId,
    string Remark);
