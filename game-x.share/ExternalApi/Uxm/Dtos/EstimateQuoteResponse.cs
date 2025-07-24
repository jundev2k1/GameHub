namespace game_x.share.ExternalApi.Uxm.Dtos;

public record EstimateQuoteResponse(
    int Direction,
    int FiatType,
    decimal FiatAmount,
    int CryptoType,
    decimal CryptoAmount,
    decimal ExchangeRate,
    decimal Fee);