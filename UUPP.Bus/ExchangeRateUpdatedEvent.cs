namespace UUPP.Bus;

public class ExchangeRateUpdatedEvent
{
    public string CryptoSymbol { get; set; } = String.Empty;
    public string FiatCode { get; set; } = String.Empty;
    public decimal SpotPrice { get; set; }
}