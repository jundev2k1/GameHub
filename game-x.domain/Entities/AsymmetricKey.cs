namespace game_x.domain.Entities;

public sealed class AsymmetricKey : BaseEntity<int>
{
    public string Name { get; private set; } = default!;
    public AsymmetricKeyType KeyType { get; private set; }
    public string Algorithm { get; private set; } = AsymmetricType.ECDSA;
    public string KeyValue { get; private set; } = default!;
    public string Description { get; private set; } = string.Empty;

    public static AsymmetricKey Create(
        string name,
        AsymmetricKeyType keyType,
        string algorithm,
        string value,
        string? desc)
    {
        return new AsymmetricKey
        {
            Name = name,
            KeyType = keyType,
            Algorithm = algorithm,
            KeyValue = value,
            Description = desc ?? string.Empty
        };
    }
}
