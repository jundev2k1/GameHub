namespace game_x.domain.ValueObjects;

public sealed class ObjectName
{
    public string Value { get; }

    private ObjectName(string value) => Value = value;

    // Derived properties
    public string FileName => Path.GetFileName(Value);
    public string Prefix => Value[..Value.LastIndexOf('/')];
    public string Extension => Path.GetExtension(Value);

    // Factories
    public static ObjectName KycProfile(string userId, string fileName)
        => Of($"user-kyc/{userId:N}/{fileName}");
    public static ObjectName BankAccountProfile(string userId, string fileName)
        => Of($"user-bank-account/{userId:N}/{fileName}");

    public static ObjectName Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Object name must be not empty.", nameof(value));

        if (ValidPrefixValues.Any(prefix => value.StartsWith($"{prefix}/")) == false)
            throw new ArgumentException($"Prefix invalid ({value}).");
        if (ValidExtensions.Any(extension => value.EndsWith($"{extension}")) == false)
            throw new ArgumentException($"Extension invalid ({value}).");

        return new ObjectName(value);
    }

    // Constants
    private static readonly string[] ValidExtensions = [".jpg", ".jpeg", ".png", ".webp"];
    private static readonly string[] ValidPrefixValues = ["user-kyc", "user-bank-account"];

    // Value object overrides
    public override bool Equals(object? obj) =>
        (obj != null) && (obj is ObjectName type) && (Value == type.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
