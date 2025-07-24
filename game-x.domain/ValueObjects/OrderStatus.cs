namespace game_x.domain.ValueObjects;

public sealed class OrderStatus
{
    private OrderStatus(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static OrderStatus Created => Of("CREATED");
    public static OrderStatus Completed => Of("COMPLETED");
    public static OrderStatus Approved => Of("APPROVED");
    public static OrderStatus Cancelled => Of("CANCELLED");

    private static readonly HashSet<string> ValidValues =
        ["CREATED", "COMPLETED", "APPROVED", "CANCELLED"];
    
    public static OrderStatus Of(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));
        var upper = value.ToUpperInvariant();
        if (!IsValid(upper))
            throw new ArgumentException($"OrderStatus '{value}' is invalid.");

        return new OrderStatus(upper);
    }

    public static bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        return ValidValues.Contains(value.ToUpperInvariant());
    }

    public override bool Equals(object? obj) =>
        (obj != null) && (obj is OrderStatus status) && (Value == status.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
