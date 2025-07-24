using System.Reflection;

namespace game_x.domain.ValueObjects;

public sealed class AccountStatus
{
    public string Value { get; }

    private AccountStatus(string value) => Value = value;

    public static AccountStatus Of(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));

        var upper = value.ToUpperInvariant();
        if (!IsValid(value))
            throw new ArgumentException($"AccountStatus '{value}' is invalid.");

        return new AccountStatus(upper);
    }

    public static bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var upper = value.ToUpperInvariant();
        var isExist = typeof(BankAccountStatus).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Any(f => f.GetValue(null)?.ToString() == upper);
        return isExist;
    }

    public override bool Equals(object? obj) =>
        (obj != null) && (obj is AccountStatus type) && (Value == type.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
