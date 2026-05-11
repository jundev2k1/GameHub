using System.ComponentModel.DataAnnotations;

namespace game_x.domain.Entities.Missions;

/// <summary>
/// Current user mission snapshot.
/// Why needed: avoid recomputing from user_events.
/// </summary>
public sealed class Inventory : BaseEntity<int>
{
    #region Identities

    [MaxLength(36)]
    public string UserId { get; private set; } = string.Empty;

    public int InventoryItemDefinitionId { get; private set; }

    #endregion

    #region Properties

    public int Quantity { get; private set; }

    #endregion

    #region Relationships
    public User? User { get; private set; }

    public InventoryItemDefinition? Item { get; private set; }
    #endregion

    #region Initializations
    private Inventory() { }

    public static Inventory Create(string userId, int inventoryItemDefinitionId, int quantity = 0)
    {
        if (quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        return new()
        {
            UserId = userId,
            InventoryItemDefinitionId = inventoryItemDefinitionId,
            Quantity = quantity
        };
    }
    #endregion

    #region Behaviors
    public void Add(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        Quantity += quantity;
    }

    public void Remove(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        if (Quantity < quantity)
            throw new InvalidOperationException("Insufficient inventory quantity.");

        Quantity -= quantity;
    }

    public void SetQuantity(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        Quantity = quantity;
    }
    #endregion
}