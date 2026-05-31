using System.Text.Json.Serialization;

namespace game_x.domain.Enum.Rewards;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum IdempotencyStatus
{
    Processing,
    Completed,
    Failed
}