using System.Text.Json.Serialization;

namespace game_x.domain.Enum.Rewards;

[Newtonsoft.Json.JsonConverter(typeof(JsonStringEnumConverter))]
public enum MissionProgressMode
{
    Count,
    SumValue
}