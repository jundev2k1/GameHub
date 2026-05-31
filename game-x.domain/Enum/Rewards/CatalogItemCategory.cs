using System.Text.Json.Serialization;

namespace game_x.domain.Enum.Rewards;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CatalogItemCategory
{
    /// <summary>Wallet-like virtual currency.</summary>
    Currency = 1,
    /// <summary>Entry token for reward participation.</summary>
    Ticket = 2,
    FreeSpin = 3,
    /// <summary>Livestream gift item.</summary>
    Gift = 4
}