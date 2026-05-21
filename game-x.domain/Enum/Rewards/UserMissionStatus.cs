using System.Text.Json.Serialization;

namespace game_x.domain.Enum.Rewards;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserMissionStatus
{
    InProgress,
    Completed,
    Claimed,
    Expired
}