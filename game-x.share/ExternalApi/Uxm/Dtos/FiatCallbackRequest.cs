namespace game_x.share.ExternalApi.Uxm.Dtos;

public record FiatCallbackRequest(
    string OrderUid,
    string OrderNumber,
    DateTime CreatedAt,
    decimal FiatAmount,
    string FiatType);
