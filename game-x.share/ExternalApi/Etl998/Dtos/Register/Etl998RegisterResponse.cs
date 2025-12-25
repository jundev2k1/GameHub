using System.Text.Json.Serialization;

namespace game_x.share.ExternalApi.Etl998.Dtos.Register;

public sealed class Etl998RegisterResponse
{
    [JsonPropertyName("account")]
    public string Account { get; set; } = string.Empty;
    [JsonPropertyName("pwd")]
    public string Password { get; set; } =  String.Empty;
}