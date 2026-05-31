using System.Text.Json.Serialization;

namespace game_x.domain.Enum.Rewards;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CatalogItemIconType
{
    /// <summary>Display emoji icon.</summary>
    Emoji,
    /// <summary>Uploaded media asset.</summary>
    File
}