namespace game_x.domain.ValueObjects;

public sealed class UserStatus
{
    public string Value { get; }

    private UserStatus(string value) => Value = value;

    public static UserStatus Active => Of("ACTIVE");
    public static UserStatus InActive => Of("INACTIVE");
    public static UserStatus Pending => Of("PENDING");
    public static UserStatus Maintenance => Of("MAINTENANCE");
    

    public static UserStatus Of(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));

        var upper = value.ToUpperInvariant();
        if (upper != "ACTIVE"
            && upper != "INACTIVE"
            && upper != "PENDING"
            && upper != "MAINTENANCE")
            throw new ArgumentException($"UserStatus '{value}' is invalid.");

        return new UserStatus(upper);
    }

    public override bool Equals(object? obj) =>
        (obj != null) && (obj is UserStatus status) && (Value == status.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
