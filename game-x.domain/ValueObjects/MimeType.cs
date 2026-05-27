namespace game_x.domain.ValueObjects;

public sealed class MimeType
{
    public string Value { get; }

    private MimeType(string value) => Value = value;

    public static MimeType Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("MIME type cannot be null or empty.", nameof(value));

        if (ValidValues.Contains(value) == false)
            throw new ArgumentException($"MIME Type is invalid ({value}).", nameof(value));

        return new MimeType(value);
    }

    private static readonly string[] ValidValues = ["image/jpeg", "image/png", "image/webp", "image/gif", "video/mp4", "video/x-matroska", "video/x-msvideo", "video/quicktime"];

    public override bool Equals(object? obj) =>
        (obj != null) && (obj is MimeType type) && (Value == type.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
