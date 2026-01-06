namespace game_x.domain.Entities;

public sealed class S2SCredentialMaterial : BaseEntity<int>
{
    public int CredentialId { get; private set; }
    public S2SCredential Credential { get; private set; } = default!;
    public CredentialMaterialType Type { get; private set; }

    public string Value { get; private set; } = string.Empty;

    public bool IsEncrypted { get; private set; }

    public static S2SCredentialMaterial Create(
        CredentialMaterialType type,
        string value,
        bool isEncrypted = false)
    {
        return new S2SCredentialMaterial
        {
            Type = type,
            Value = value,
            IsEncrypted = isEncrypted
        };
    }
}
