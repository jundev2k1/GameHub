namespace game_x.domain.ValueObjects;

public sealed class AuditSource
{
    public string Value { get; }

    private AuditSource(string value) => Value = value;

    public static AuditSource WebApi => Of("WebApi");
    public static AuditSource Scheduler => Of("Scheduler");
    public static AuditSource ImportJob => Of("ImportJob");
    public static AuditSource External => Of("External");
    public static AuditSource Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Audit Source cannot be null or empty.", nameof(value));

        if (ValidValues.Contains(value) == false)
            throw new ArgumentException($"Audit Source is invalid ({value}).", nameof(value));

        return new AuditSource(value);
    }

    private static readonly string[] ValidValues = ["WebApi", "Scheduler", "ImportJob", "External"];

    public override bool Equals(object? obj) =>
        (obj != null) && (obj is AuditSource type) && (Value == type.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
