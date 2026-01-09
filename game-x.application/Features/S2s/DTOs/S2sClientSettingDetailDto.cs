using System.Text.Json.Serialization;

namespace game_x.application.Features.S2s.DTOs;

public sealed class S2sClientSettingDetailDto
{
    [JsonIgnore]
    public int LocalId { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ClientCode { get; set; } = string.Empty;
    public bool IsClientActive { get; set; }
    public string ClientNotes { get; set; } = string.Empty;
    public string AppCode { get; set; } = string.Empty;
    public string AppName { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    [JsonIgnore]
    public string AllowedIpsString { get; set; } = string.Empty;
    public string[] AllowedIps => AllowedIpsString.Split(",");
    public bool IsSettingActive { get; set; }
    public string SettingNotes { get; set; } = string.Empty;
    public S2sCredentialDto[] Credentials { get; set; } = [];
    public DateTime ClientCreatedAt { get; set; }
    public DateTime ClientUpdatedAt { get; set; }
    public DateTime SettingCreatedAt { get; set; }
    public DateTime SettingUpdatedAt { get; set; }
}
