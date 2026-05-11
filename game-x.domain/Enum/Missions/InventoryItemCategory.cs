using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace game_x.domain.Enum.Missions;

[JsonConverter(typeof(StringEnumConverter))]
public enum InventoryItemCategory
{
    /// <summary>Wallet-like virtual currency.</summary>
    Currency,
    /// <summary>Entry token for reward participation.</summary>
    Ticket,
    FreeSpin,
    /// <summary>Livestream gift item.</summary>
    Gift
}