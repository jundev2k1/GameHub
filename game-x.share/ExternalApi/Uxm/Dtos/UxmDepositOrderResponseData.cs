namespace game_x.share.ExternalApi.Uxm.Dtos;

public record UxmDepositOrderResponseData(

    string OrderUid,
    decimal Amount,
    string To);
