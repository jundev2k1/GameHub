using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace game_x.domain.Enum.Missions;

[JsonConverter(typeof(StringEnumConverter))]
public enum UserEventRefType
{
    Mission,
    RewardPool,
    Execution,
    Transaction,
    ShareLink,
    Reward
}