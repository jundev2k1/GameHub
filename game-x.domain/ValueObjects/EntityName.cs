namespace game_x.domain.ValueObjects;

public sealed class EntityName
{
    public string Value { get; }

    private EntityName(string value) => Value = value;

    public static EntityName User => Of(nameof(Entities.User));
    public static EntityName MediaFile => Of(nameof(Entities.MediaFile));
    public static EntityName UserKyc => Of(nameof(Entities.UserKyc));
    public static EntityName ChainTransaction => Of(nameof(Entities.ChainTransaction));
    public static EntityName GameTransaction => Of(nameof(Entities.GameTransaction));
    public static EntityName UserBalance => Of(nameof(Entities.UserBalance));

    public static EntityName Of(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace("EntityName cannot be null or empty.", nameof(value));

        if (ValidValues.Contains(value) == false)
            throw new ArgumentException("Entity name is not exists.", nameof(value));

        return new EntityName(value);
    }

    private static readonly string[] ValidValues = [
        nameof(Entities.User),
        nameof(Entities.MediaFile),
        nameof(Entities.UserKyc),
        nameof(Entities.ChainTransaction),
        nameof(Entities.GameTransaction),
        nameof(Entities.UserBalance)];

    public override bool Equals(object? obj) =>
        (obj != null) && (obj is EntityName type) && (Value == type.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}