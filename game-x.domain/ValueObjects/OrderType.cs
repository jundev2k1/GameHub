namespace game_x.domain.ValueObjects;

public sealed class OrderType
{
    public string Value { get; }
    public int UxmValue { get; }

    private static string BuyValue => "BUY";
    private static string SellValue => "SELL";
    private static int UxmBuyValue => 0;
    private static int UxmSellValue => 1;
    public static OrderType Buy => Of(BuyValue);
    public static OrderType Sell => Of(SellValue);
    private static readonly HashSet<string> ValidValues = [BuyValue, SellValue];
    
    private OrderType(string value)
    {
        Value = value;
        UxmValue = GetUxmValue(value);
    }

    private int GetUxmValue(string value)
    {
        if(value == BuyValue)
            return UxmBuyValue;
        if(value == SellValue)
            return UxmSellValue;
        throw new ArgumentException($"OrderType must be '{BuyValue}' or '{SellValue}'.");
    }
    
    public static OrderType Of(string value)
    {
        if (!IsValid(value))
            throw new ArgumentException($"OrderType must be '{BuyValue}' or '{SellValue}'.");
        return new OrderType(value.ToUpperInvariant());
    }

    public override bool Equals(object? obj) =>
        (obj != null) && (obj is OrderType type) && (Value == type.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
    
    public static bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        return ValidValues.Contains(value.ToUpperInvariant());
    }
    
    public static OrderType GetOrderType(int value)
    {
        if(value == UxmBuyValue)
            return Buy;
        if(value == UxmSellValue)
            return Sell;
        throw new ArgumentException();
    }
}
