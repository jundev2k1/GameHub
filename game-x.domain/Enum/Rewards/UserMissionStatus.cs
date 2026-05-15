using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace game_x.domain.Enum.Rewards;

[JsonConverter(typeof(StringEnumConverter))]
public enum UserMissionStatus
{
    InProgress,
    Completed,
    Claimed,
    Expired
}