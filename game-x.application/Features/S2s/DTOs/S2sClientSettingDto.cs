using System.Text.Json.Serialization;

namespace game_x.application.Features.S2s.DTOs;

public sealed class S2sClientSettingDto
{
    [JsonIgnore]
    public int LocalId { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string AppCode { get; private set; } = string.Empty;
    public string AppName { get; private set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
    public string[] AllowIps { get; set; } = [];
    public bool IsActive { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
