using System.Reflection;

namespace game_x.domain.ValueObjects;

public sealed class CurrencyUnit
{
    public string Value { get; }

    private CurrencyUnit(string value) => Value = value;

    public static CurrencyUnit Of(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));

        var upper = value.ToUpperInvariant();
        if (!IsValid(upper))
            throw new ArgumentException($"CurrencyUnit '{value}' is invalid.");

        return new CurrencyUnit(upper);
    }

    public static bool IsValid(string value)
    {
        var upper = value.ToUpperInvariant();
        var isExist = typeof(CurrencyCode).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Any(f => f.GetValue(null)?.ToString() == upper);
        return isExist;
    }

    public override bool Equals(object? obj) =>
        (obj != null) && (obj is CurrencyUnit type) && (Value == type.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
