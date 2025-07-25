namespace game_x.domain.ValueObjects;

public sealed class EntityName
{
    public string Value { get; }

    private EntityName(string value) => Value = value;

    public static EntityName User => Of(nameof(Entities.User));
    public static EntityName MediaFile => Of(nameof(Entities.MediaFile));

    public static EntityName Of(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace("EntityName cannot be null or empty.", nameof(value));

        if (ValidValues.Contains(value) == false)
            throw new ArgumentException("Entity name is not exists.", nameof(value));

        return new EntityName(value);
    }

    private static readonly string[] ValidValues = [
        nameof(Entities.User),
        nameof(Entities.MediaFile)];

    public override bool Equals(object? obj) =>
        (obj != null) && (obj is EntityName type) && (Value == type.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
