using System.Text.Json.Serialization;

namespace game_x.domain.Enum.Rewards;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum IdempotencyActionType
{
    /// <summary>Prevent duplicate spin.</summary>
    Spin,
    /// <summary>Prevent duplicate mission claim.</summary>
    MissionClaim,
    /// <summary>Prevent duplicate reward grant.</summary>
    RewardGrant,
    /// <summary>Prevent duplicate share validation.</summary>
    ShareValidation,
    /// <summary>Prevent duplicate payment callback.</summary>
    Deposit
}