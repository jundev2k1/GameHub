using System.Text.Json.Serialization;

namespace game_x.domain.Enum.Rewards;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExecutionType
{
    /// <summary>Lucky wheel / gacha / scratch execution.</summary>
    Spin,
    /// <summary>Mission progress update.</summary>
    MissionProgress,
    /// <summary>Mission reward claim.</summary>
    MissionRewardClaim,
    /// <summary>Reward distribution execution.</summary>
    RewardGrant,
    /// <summary>Social share validation.</summary>
    ShareValidation,
    /// <summary>Manual admin compensation.</summary>
    AdminGrant
}