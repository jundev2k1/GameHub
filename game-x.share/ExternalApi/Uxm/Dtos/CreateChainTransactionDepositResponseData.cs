namespace game_x.share.ExternalApi.Uxm.Dtos;

public record CreateChainTransactionDepositResponseData(

    string OrderUid,
    decimal Amount,
    string To);
