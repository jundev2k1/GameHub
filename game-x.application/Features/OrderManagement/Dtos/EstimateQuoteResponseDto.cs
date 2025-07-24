namespace game_x.application.Features.OrderManagement.Dtos;

public sealed class EstimateQuoteResponseDto
{
    public string OrderType { get; set; } = string.Empty;
    public FiatType FiatType { get; set; }
    public decimal FiatAmount { get; set; }
    public CryptoType CryptoType { get; set; }
    public decimal CryptoAmount { get; set; }
    public decimal ExchangeRate { get; set; }
    public decimal UxmFee { get; set; }
}
