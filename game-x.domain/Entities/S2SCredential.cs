namespace game_x.domain.Entities;

public sealed class S2SCredential : BaseEntity<int>
{
    public int SettingId { get; private set; }
    public S2SClientSetting ClientSetting { get; private set; } = default!;

    public string KeyId { get; private set; } = string.Empty;
    public CredentialDirection Direction { get; private set; }
    public AuthMethod AuthMethod { get; private set; }
    public KeyUsageScope UsageScope { get; private set; }
    public CredentialStatus Status { get; private set; }
    public DateTime ActivatedAt { get; private set; }
    public DateTime? ExpiredAt { get; private set; }

    public ICollection<S2SCredentialMaterial> Materials { get; private set; } = [];

    public static S2SCredential Create(
        AuthMethod authMethod,
        CredentialDirection direction,
        KeyUsageScope usageScope)
    {
        return new S2SCredential
        {
            KeyId = GenerateKeyId(),
            AuthMethod = authMethod,
            Direction = direction,
            UsageScope = usageScope,
            Status = CredentialStatus.Active,
            ActivatedAt = DateTime.UtcNow
        };
    }

    public void Deactivate()
    {
        Status = CredentialStatus.Inactive;
        ExpiredAt = DateTime.UtcNow;
    }

    private static string GenerateKeyId()
    {
        return $"k_{Guid.CreateVersion7():N}";
    }
}
