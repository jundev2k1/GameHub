using System.Reflection;

namespace game_x.domain.ValueObjects;

public sealed class GameTagColor
{
    public string Value { get; }

    private GameTagColor(string value) => Value = value;

    public static GameTagColor Of(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));

        var upper = value.ToLowerInvariant();
        if (!IsValid(upper))
            throw new ArgumentException($"Game Tag Color '{value}' is invalid.");

        return new GameTagColor(upper);
    }

    public static bool IsValid(string value)
    {
        var upper = value.ToLowerInvariant();
        var isExist = typeof(GameTagColors).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Any(f => f.GetValue(null)?.ToString() == upper);
        return isExist;
    }

    public override bool Equals(object? obj) =>
        (obj != null) && (obj is GameTagColor type) && (Value == type.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
