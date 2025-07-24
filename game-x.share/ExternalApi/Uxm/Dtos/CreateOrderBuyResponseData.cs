namespace game_x.share.ExternalApi.Uxm.Dtos;

public record CreateOrderBuyResponseData(
    string EntryCode,
    string RedirectUrl,
    string OrderUid,
    decimal FiatAmount,
    string FiatType);
