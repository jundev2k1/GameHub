using System.Text.Json.Serialization;

namespace game_x.domain.Enum.Rewards;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MissionResetType
{
    /// <summary>Reset every day.</summary>
    Daily,
    /// <summary>Reset every week.</summary>
    Weekly,
    /// <summary>Reset every month.</summary>
    Monthly,
    /// <summary>One-time mission.</summary>
    Never
}