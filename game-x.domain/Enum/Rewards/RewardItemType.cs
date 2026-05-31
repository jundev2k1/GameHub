using System.Text.Json.Serialization;

namespace game_x.domain.Enum.Rewards;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RewardItemType
{
    /// <summary>No reward slot / empty result.</summary>
    None,
    /// <summary>Wallet / points reward.</summary>
    Balance,
    /// <summary>Inventory item reward.</summary>
    CatalogItem,
    /// <summary>Externally fulfilled reward.</summary>
    External
}