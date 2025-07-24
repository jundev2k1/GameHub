namespace game_x.share.ExternalApi.Uxm.Dtos;

public record CreateSellOrderV2Response(
    string EntryCode,
    string? RedirectUrl,
    string OrderUid,
    decimal FiatAmount,
    decimal CryptoAmount,
    int FiatType,
    int CryptoType,
    decimal Fee,
    long Timestamp);