namespace game_x.domain.Entities;

public class AsymmetricKey : BaseEntity<int>
{
    public string Name { get; set; } = default!;

    public KeyType KeyType { get; set; }

    public string Algorithm { get; set; } = AsymmetricType.ECDSA;

    public string KeyValue { get; set; } = default!;

    public string? Description { get; set; }
}

public enum KeyType
{
    Public,
    Private
}