using System.Text.Json.Serialization;

namespace game_x.domain.Enum.Missions;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserEventType
{
    DailyLogin,
    Deposit,
    Share,
    Spin
}