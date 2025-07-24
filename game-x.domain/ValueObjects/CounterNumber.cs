namespace game_x.domain.ValueObjects;

public sealed class CounterNumber
{
    public string Value { get; }

    private CounterNumber(string id) => Value = id;

    public static CounterNumber Of(string id)
    {
        ArgumentException.ThrowIfNullOrEmpty(id, nameof(id));

        if (id.Length != 4)
            throw new ArgumentException(id, nameof(id));

        return new CounterNumber(id);
    }

    public override bool Equals(object? obj)
        => (obj != null) && (obj is CounterNumber type) && (Value == type.Value);

    public override int GetHashCode() => Value.GetHashCode();
}
