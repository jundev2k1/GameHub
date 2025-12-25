using System.Text.Json.Serialization;

namespace game_x.share.ExternalApi.Etl998.Dtos.ModifyBettingLimit;

public sealed class ModifyBettingLimitRequest
{
    [JsonPropertyName("account")]
    public required string Account { get; set; }
    [JsonPropertyName("tables")]
    public required string Tables { get; set; } = "1,2,3";
}