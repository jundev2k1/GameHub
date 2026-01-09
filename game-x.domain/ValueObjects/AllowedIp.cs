namespace game_x.domain.ValueObjects;

public sealed class AllowedIp
{
    public string Value { get; }

    private AllowedIp(string value) => Value = value;

    public static AllowedIp Of(string value)
    {
        if (!IsValid(value))
            throw new ArgumentException($"AllowedIp '{value}' is invalid.");

        return new AllowedIp(value);
    }

    public static bool IsValid(string value)
    {
        if (string.IsNullOrEmpty(value))
            return true;

        var ips = value.Split(",");
        var isValid = ips.Distinct().Count() == ips.Length
            && ips.All(ip => ip.IsNotNullOrEmpty());
        return isValid;
    }

    public override bool Equals(object? obj) =>
        (obj != null) && (obj is AllowedIp type) && (Value == type.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    public string[] GetAllowIps() => Value.Split(",");
}
