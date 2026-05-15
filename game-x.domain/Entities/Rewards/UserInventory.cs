using System.ComponentModel.DataAnnotations;

namespace game_x.domain.Entities.Rewards;

/// <summary>
/// Current user mission snapshot.
/// Why needed: avoid recomputing from user_events.
/// </summary>
public sealed class UserInventory : BaseEntity<int>
{
    #region Identities

    [MaxLength(36)]
    public string UserId { get; private set; } = string.Empty;

    public int CatalogItemId { get; private set; }

    #endregion

    #region Properties

    public int Quantity { get; private set; }

    #endregion

    #region Relationships
    public User? User { get; init; }

    public CatalogItem? Item { get; init; }
    #endregion

    #region Initializations
    private UserInventory() { }

    public static UserInventory Create(string userId, int catalogItemId, int quantity = 0)
    {
        if (quantity < 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));

        return new()
        {
            UserId = userId,
            CatalogItemId = catalogItemId,
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

    public void Deduct(int quantity)
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