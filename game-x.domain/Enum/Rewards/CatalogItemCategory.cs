using System.Text.Json.Serialization;

namespace game_x.domain.Enum.Rewards;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CatalogItemCategory
{
    /// <summary>Wallet-like virtual currency.</summary>
    Currency,
    /// <summary>Entry token for reward participation.</summary>
    Ticket,
    FreeSpin,
    /// <summary>Livestream gift item.</summary>
    Gift
}