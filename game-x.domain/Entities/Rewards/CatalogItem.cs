using System.ComponentModel.DataAnnotations;
using game_x.domain.Enum.Rewards;

namespace game_x.domain.Entities.Rewards;

/// <summary>
/// Master catalog of inventory items.
/// Purpose: define all reusable item types.
/// Why needed: avoid hardcoded inventory types.
/// </summary>
public sealed class CatalogItem: BaseEntity<int>, IAuditable
{
    #region Indentities
    public Guid PublicId { get; private set; } = Guid.CreateVersion7();

    [MaxLength(128)]
    public string Code { get; private set; } = string.Empty;
    #endregion

    #region Properties
    [MaxLength(256)]
    public string Name { get; private set; } = string.Empty;

    [MaxLength(4096)]
    public string? Description { get; private set; }
    
    public CatalogItemCategory Category { get; private set; }
    
    /// <summary>
    /// Monetary/business value of the item.
    /// Example: ROSE_GIFT = 1, DIAMOND_GIFT = 100
    /// </summary>
    public decimal? MonetaryValue { get; private set; }
    
    /// <summary>Emoji value like 🎟️ or 🌹.</summary>
    public CatalogItemIconType IconType { get; private set; } = CatalogItemIconType.Emoji;
    
    public int? IconId { get; private set; }
    
    [MaxLength(2048)]
    public string? IconValue { get; private set; }
    
    public bool IsActive { get; private set; } = true;
    
    public int SortOrder { get; private set; }
    
    public DateTime? DeletedAt { get; private set; }
    #endregion

    #region Relationships
    public MediaFile? Icon { get; private set; }
    
    private readonly List<RewardDefinition> _rewardDefinitions = [];
    public IReadOnlyCollection<RewardDefinition> RewardDefinitions => _rewardDefinitions;
    #endregion                                                                                                   
    
    #region Initializations
    public static CatalogItem Create(
        string code,
        string name,
        CatalogItemCategory category,
        int sortOrder,
        CatalogItemIconType? iconType = null,
        string? description = null,
        decimal? monetaryValue = null,
        string? iconValue = null,
        Guid? id = null)
    {
        return new()
        {
            PublicId = id ?? Guid.CreateVersion7(),
            Code = code,
            Name = name,
            Description = description,
            Category = category,
            MonetaryValue = monetaryValue,
            IconType = iconType ?? CatalogItemIconType.Emoji,
            IconValue = iconValue,
            SortOrder = sortOrder,
        };
    }
    #endregion

    #region Behaviors
    public void OnUpdateIcon(int id)
    {
        IconId = id;
    }
    
    public void OnUpdateIcon(MediaFile icon)
    {
        Icon = icon;
    }
    
    public void OnUpdate(
        string? code = null,
        string? name = null,
        string? description = null,
        CatalogItemCategory? category = null,
        decimal? monetaryValue = null,
        CatalogItemIconType? iconType = null,
        string? iconValue = null,
        bool? isActive = null,
        int? sortOrder = null)
    {
        Code = code ?? Code;
        Name = name ?? Name;
        Description = description ?? Description;
        Category = category ?? Category;
        MonetaryValue = monetaryValue ?? MonetaryValue;
        IsActive = isActive ?? IsActive;
        SortOrder = sortOrder ?? SortOrder;
        IconType = iconType ?? IconType;
        IconValue = iconValue ?? IconValue;
    }
    
    public void SoftDelete()
    {
        DeletedAt = DateTime.UtcNow;
    }
    #endregion
}