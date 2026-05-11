using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace game_x.domain.Enum.Missions;

[JsonConverter(typeof(StringEnumConverter))]
public enum InventoryIconType
{
    /// <summary>Display emoji icon.</summary>
    Emoji,
    /// <summary>Uploaded media asset.</summary>
    File
}