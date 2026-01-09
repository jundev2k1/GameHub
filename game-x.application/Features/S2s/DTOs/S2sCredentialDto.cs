using System.Text.Json.Serialization;

namespace game_x.application.Features.S2s.DTOs;

public sealed class S2sCredentialDto
{
    [JsonIgnore]
    public int SettingId { get; set; }
    public string KeyId { get; set; } = string.Empty;
    public CredentialDirection Direction { get; set; }
    public AuthMethod AuthMethod { get; set; }
    public KeyUsageScope UsageScope { get; set; }
    public CredentialStatus Status { get; set; }
    public DateTime ActivatedAt { get; set; }
    public DateTime? ExpiredAt { get; set; }
    public S2sCredentialMaterialDto[] Materials { get; set; } = [];
}

public sealed class S2sCredentialMaterialDto
{
    [JsonIgnore]
    public int CredentialId { get; private set; }
    public CredentialMaterialType Type { get; private set; }
    public string Value { get; private set; } = string.Empty;
    public bool IsEncrypted { get; private set; }
}
