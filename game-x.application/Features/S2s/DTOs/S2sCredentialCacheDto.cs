namespace game_x.application.Features.S2s.DTOs;

public sealed class S2sCredentialCacheDto
{
    public static S2sCredentialCacheDto? Create(S2sCredentialDto[] input)
    {
        var firstItem = input.FirstOrDefault();
        if (firstItem == null) return null;

        return new S2sCredentialCacheDto
        {
            Item = firstItem
        };
    }

    private S2sCredentialDto[] DataSource { get; init; } = [];
    private S2sCredentialDto Item { get; init; } = default!;

    public int SettingId => Item.SettingId;
    public string KeyId => Item.KeyId;
    public CredentialDirection Direction => Item.Direction;
    public AuthMethod AuthMethod => Item.AuthMethod;
    public KeyUsageScope UsageScope => Item.UsageScope;
    public CredentialStatus Status => Item.Status;
    public DateTime ActivatedAt => Item.ActivatedAt;
    public DateTime? ExpiredAt => Item.ExpiredAt;
    public S2sCredentialMaterialDto[] InBound => DataSource
        .FirstOrDefault(i => i.Direction == CredentialDirection.Inbound)
        ?.Materials
        .Adapt<S2sCredentialMaterialDto[]>().ToArray() ?? [];
    public S2sCredentialMaterialDto[] OutBound => DataSource
        .FirstOrDefault(i => i.Direction == CredentialDirection.Outbound)
        ?.Materials
        .Adapt<S2sCredentialMaterialDto[]>().ToArray() ?? [];
}
