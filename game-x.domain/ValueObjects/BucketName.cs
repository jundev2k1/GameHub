namespace game_x.domain.ValueObjects;

public sealed class BucketName
{
    public string Value { get; }

    private BucketName(string value) => Value = value;

    public static BucketName User => Of("user");
    public static BucketName Chat => Of("chat");
    public static BucketName Game => Of("game");
    public static BucketName LiveStream => Of("live-stream");
    public static BucketName Interaction => Of("interaction");

    public static BucketName Of(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace("Bucket name cannot be null or empty.", nameof(value));

        if (ValidValues.Contains(value) == false)
            throw new ArgumentException("Invalid bucket name.", nameof(value));

        return new BucketName(value);
    }

    private static readonly string[] ValidValues = ["user", "chat", "game", "live-stream", "interaction"];

    public override bool Equals(object? obj) =>
        (obj != null) && (obj is BucketName type) && (Value == type.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
