using System.Text.Json.Serialization;

namespace game_x.share.ExternalApi.Etl998.Dtos.ForwardGame;

public sealed class ForwardGameRequest
{
    [JsonPropertyName("account")]
    public required string Account { get; set; }
    [JsonPropertyName("pwd")]
    public required string Password { get; set; } = string.Empty;
    /// <summary>Game URL provided by the platform (interface game address).</summary>
    [JsonPropertyName("dm")]
    public required string Dm { get; set; } = string.Empty;
}