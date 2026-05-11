using System.Text.Json.Serialization;

namespace game_x.domain.Enum.Missions;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RewardPoolType
{
    Roulette, // Spin wheel reward game
    Scratch, // Scratch card reveal game
    Gacha // Loot-box style random draw
}