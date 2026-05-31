using System.Reflection;

namespace game_x.domain.ValueObjects;

public sealed class LanguageCode
{
    public string Value { get; }

    private LanguageCode(string value) => Value = value;

    public static LanguageCode Of(string value)
    {
        if (value.IsNotNullOrEmpty() && !IsValid(value))
            throw new ArgumentException($"Language Code '{value}' is invalid.");

        return new LanguageCode(value);
    }

    public static bool IsValid(string value)
    {
        if (value.IsNullOrEmpty()) return true;

        var isExist = typeof(domain.Constants.LanguageCodeValue).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Any(f => f.GetValue(null)?.ToString() == value);
        return isExist;
    }

    public override bool Equals(object? obj) =>
        (obj != null) && (obj is LanguageCode type) && (Value == type.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
