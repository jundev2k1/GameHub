namespace game_x.application.Features.CounterManagement.Dtos;

public sealed class CounterStatisticsDto
{
    public int TotalOrders { get; set; }
    public int TotalBuyOrders { get; set; }
    public int TotalSellOrders { get; set; }
    public decimal BuyFiatAmount { get; set; }
    public decimal BuyCryptoAmount { get; set; }
    public decimal SellFiatAmount { get; set; }
    public decimal SellCryptoAmount { get; set; }
    public decimal Profit { get; set; }
    public decimal TotalUxmBuyFee { get; set; } // The total transaction fee for the selected cryptocurrency. (e.g., BTC Fee)
    public decimal TotalUxmSellFee { get; set; } // The total transaction fee for the selected cryptocurrency. (e.g., BTC Fee)
}