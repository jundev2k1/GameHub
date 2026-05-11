using System.ComponentModel.DataAnnotations;
using game_x.domain.Enum.Missions;

namespace game_x.domain.Entities.Missions;

/// <summary>
/// Reward outcomes inside a reward pool.
/// Why needed: weighted random outcome engine.
/// </summary>
public sealed class RewardItem : BaseEntity<int>, IAuditable
{
    #region Indentities
    public Guid PublicId { get; private set; } = Guid.CreateVersion7();
    
    public int RewardPoolId { get; private set; }
    
    /// <summary>Example: BONUS_100, TICKET_1, FREE_SPIN, JACKPOT_10000.</summary>
    [MaxLength(128)]
    public string Code { get; private set; } = string.Empty;
    
    [MaxLength(256)]
    public string? InventoryItemCode { get; private set; }
    
    public InventoryItemDefinition? Item { get; private set; }
    #endregion

    #region Properties
    public RewardItemType Type { get; private set; }

    [MaxLength(256)]
    public string Title { get; private set; } = string.Empty;

    [MaxLength(4096)]
    public string? Description { get; private set; }
    
    /// <summary>Balance amount / quantity / reward value.</summary>
    public decimal Amount { get; private set; }
    
    public int SortOrder { get; private set; }

    public bool IsActive { get; private set; } = true;
    
    /// <summary>Probability / weighted random. The higher the weight, the higher the probability of winning.</summary>
    public int Weight { get; private set; }
    
    [MaxLength(4096)]
    public string? Metadata { get; private set; }
    
    public DateTime? StartAt { get; private set; }

    public DateTime? EndAt { get; private set; }

    public DateTime? DeletedAt { get; private set; }
    #endregion

    #region Relationships
    public RewardPool? RewardPool { get; private set; }
    public ICollection<UserReward> UserRewards { get; private set; } = [];
    #endregion

    #region Initializations
    private RewardItem() { }

    public static RewardItem Create(
        int rewardConfigId,
        string code,
        RewardItemType rewardType,
        string title,
        decimal amount,
        int weight,
        string? description = null,
        string? metadata = null,
        int sortOrder = 0,
        DateTime? startAt = null,
        DateTime? endAt = null)
    {
        return new()
        {
            RewardPoolId = rewardConfigId,
            Code = code,
            Type = rewardType,
            Title = title,
            Description = description,
            Amount = amount,
            Weight = weight,
            Metadata = metadata,
            SortOrder = sortOrder,
            StartAt = startAt,
            EndAt = endAt,
            IsActive = true
        };
    }
    #endregion

    #region Behaviors
    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    public void SoftDelete() => DeletedAt = DateTime.UtcNow;
    #endregion
}