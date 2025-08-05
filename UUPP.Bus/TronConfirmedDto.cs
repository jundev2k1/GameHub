namespace UUPP.Bus;

public class TronConfirmedDto
{
    public string? Hash { get; set; }
    public string? To { get; set; }
    public string? From { get; set; }
    public decimal Amount { get; set; }
    public decimal Gas { get; set; }
    public string? ContractType { get; set; }
    public bool Result { get; set; }
    public long TimeStamp { get; set; }
    public string CryptoType { get; set; } = string.Empty;
    public string? Error { get; set; }
    public string? OrderNumber { get; set; }
}