using System.ComponentModel.DataAnnotations;
using game_x.domain.Enum.Missions;

namespace game_x.domain.Entities.Missions;

/// <summary>
/// Master catalog of inventory items.
/// Purpose: define all reusable item types.
/// Why needed: avoid hardcoded inventory types.
/// </summary>
public sealed class InventoryItemDefinition: BaseEntity<int>, IAuditable
{
    #region Indentities
    public Guid PublicId { get; private set; } = Guid.CreateVersion7();
    
    /// <summary>Examples: TICKET, FREE_SPIN, ROSE_GIFT, DIAMOND_GIFT, COUPON_50</summary>
    [MaxLength(128)]
    public string Code { get; private set; } = string.Empty;
    #endregion

    #region Properties
    [MaxLength(256)]
    public string Name { get; private set; } = string.Empty;

    [MaxLength(4096)]
    public string? Description { get; private set; }
    
    public InventoryItemCategory Category { get; private set; }
    
    /// <summary>
    /// Monetary/business value of the item.
    /// Example: ROSE_GIFT = 1, DIAMOND_GIFT = 100
    /// </summary>
    public decimal? MonetaryValue { get; private set; }
    
    /// <summary>Emoji value like 🎟️ or 🌹.</summary>
    public InventoryIconType IconType { get; private set; } = InventoryIconType.Emoji;
    
    public int? IconId { get; private set; }
    
    public MediaFile? Icon { get; private set; }
    
    [MaxLength(2048)]
    public string? IconValue { get; private set; }
    
    public bool IsActive { get; private set; } = true;
    #endregion

    #region Initializations
    public static InventoryItemDefinition Create(
        string code,
        string name,
        string? description,
        InventoryItemCategory category,
        int monetaryValue = 0)
    {
        return new()
        {
            Code = code,
            Name = name,
            Description = description,
            Category = category,
            MonetaryValue = monetaryValue
        };
    }
    #endregion
}