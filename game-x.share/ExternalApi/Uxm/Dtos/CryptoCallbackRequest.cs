namespace game_x.share.ExternalApi.Uxm.Dtos;

public record CryptoCallbackRequest(
    string OrderUid,
    string OrderNumber,
    string Hash,
    decimal ActualAmount,
    DateTime CreatedAt,
    DateTime ConfirmedAt,
    string Remark);
