namespace game_x.domain.ValueObjects;

public sealed class EntityName
{
    public string Value { get; }

    private EntityName(string value) => Value = value;

    public static EntityName User => Of(nameof(AppUser));
    public static EntityName Order => Of(nameof(Entities.Order));
    public static EntityName BankAccount => Of(nameof(Entities.BankAccount));
    public static EntityName Counter => Of(nameof(Entities.Counter));
    public static EntityName MediaFile => Of(nameof(Entities.MediaFile));
    public static EntityName Passport => Of(nameof(UserPassport));

    public static EntityName Of(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace("EntityName cannot be null or empty.", nameof(value));

        if (ValidValues.Contains(value) == false)
            throw new ArgumentException("Entity name is not exists.", nameof(value));

        return new EntityName(value);
    }

    private static readonly string[] ValidValues = [
        nameof(AppUser),
        nameof(Entities.Order),
        nameof(Entities.BankAccount),
        nameof(Entities.Counter),
        nameof(Entities.MediaFile),
        nameof(UserPassport)];

    public override bool Equals(object? obj) =>
        (obj != null) && (obj is EntityName type) && (Value == type.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
