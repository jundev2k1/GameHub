using System.Text.Json.Serialization;

namespace game_x.domain.Enum.Rewards;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MissionType
{
    /// <summary>Login progression mission.</summary>
    DailyLogin,
    /// <summary>Deposit requirement mission.</summary>
    Deposit,
    DepositAccumulation,
    /// <summary>Social sharing mission.</summary>
    Share,
    /// <summary>Future extensibility.</summary>
    Spin
}