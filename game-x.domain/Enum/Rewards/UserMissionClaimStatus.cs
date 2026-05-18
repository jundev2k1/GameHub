using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace game_x.domain.Enum.Rewards;

[JsonConverter(typeof(StringEnumConverter))]
public enum UserMissionClaimStatus
{
    Locked,
    Available,
    Claimed,
    Expired
}