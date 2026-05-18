using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace game_x.domain.Enum.Rewards;

[JsonConverter(typeof(StringEnumConverter))]
public enum CatalogItemIconType
{
    /// <summary>Display emoji icon.</summary>
    Emoji,
    /// <summary>Uploaded media asset.</summary>
    File
}