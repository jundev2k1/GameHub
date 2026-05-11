using System.Text.Json.Serialization;

namespace game_x.domain.Enum.Missions;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRewardStatus
{
    /// <summary>Reward successfully granted.</summary>
    Granted,
    /// <summary>Reward expired before use.</summary>
    Expired,
    /// <summary>Reward manually/system revoked.</summary>
    Revoked
}