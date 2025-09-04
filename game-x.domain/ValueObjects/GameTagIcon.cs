using System.Reflection;

namespace game_x.domain.ValueObjects;

public sealed class GameTagIcon
{
    public string Value { get; }

    private GameTagIcon(string value) => Value = value;

    public static GameTagIcon Of(string value)
    {
        var upper = value.ToLowerInvariant();
        if (upper.IsNotNullOrEmpty() && !IsValid(upper))
            throw new ArgumentException($"Game Tag Icon '{value}' is invalid.");

        return new GameTagIcon(upper);
    }

    public static bool IsValid(string value)
    {
        if (value.IsNullOrEmpty()) return true;

        var upper = value.ToLowerInvariant();
        var isExist = typeof(GameTagIcons).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Any(f => f.GetValue(null)?.ToString() == upper);
        return isExist;
    }

    public override bool Equals(object? obj) =>
        (obj != null) && (obj is GameTagIcon type) && (Value == type.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
