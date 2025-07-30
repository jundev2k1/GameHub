namespace game_x.share.ExternalApi.Uxm.Dtos;

public record CreateOrderBuyResponseData(

    string OrderUid,
    decimal Amount,
    string To);
