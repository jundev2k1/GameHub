using System.Text.Json.Serialization;

namespace game_x.share.ExternalApi.Etl998.Dtos.ChangePassword;

public sealed class ChangePasswordRequest
{
    [JsonPropertyName("account")]
    public required string Account { get; set; }
    [JsonPropertyName("pwd")]
    public required string Password { get; set; } = string.Empty;
}