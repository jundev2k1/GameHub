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
    public static ObjectName Avatar(string userId, string fileName)
        => Of($"avatar/{userId:N}/{fileName}");
    public static ObjectName Attachment(string userId, string fileName)
        => Of($"attachment/{userId:N}/{fileName}");
    public static ObjectName KycProfile(string userId, string fileName)
        => Of($"user-kyc/{userId:N}/{fileName}");
    public static ObjectName BankAccountProfile(string userId, string fileName)
        => Of($"user-bank-account/{userId:N}/{fileName}");
    public static ObjectName GameResource(Guid gameId, string fileName)
        => Of($"games/{gameId:N}/thumbnail/{fileName}");
    public static ObjectName LiveStreamThumbnail(Guid scheduleId, string fileName)
        => Of($"schedules/{scheduleId:N}/thumbnail/{fileName}");

    public static ObjectName LiveStreamGiftIcon(Guid giftId, string fileName)
        => Of($"gifts/{giftId:N}/icon/{fileName}");
    public static ObjectName LiveStreamGiftAnimation(Guid giftId, string fileName)
        => Of($"gifts/{giftId:N}/animation/{fileName}");

    public static ObjectName InteractionCharacter(Guid characterId, string fileName)
        => Of($"characters/{characterId:N}/poses/{fileName}");

    public static ObjectName CatalogItem(Guid catalogItemId, string fileName)
        => Of($"catalog_items/{catalogItemId:N}/icons/{fileName}");
    
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
    private static readonly string[] ValidExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif"];
    private static readonly string[] ValidPrefixValues = [
        "user-kyc",
        "user-bank-account",
        "attachment",
        "games",
        "avatar",
        "schedules",
        "gifts",
        "characters",
        "catalog_items"];

    // Value object overrides
    public override bool Equals(object? obj) =>
        (obj != null) && (obj is ObjectName type) && (Value == type.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
