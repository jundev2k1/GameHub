using System.ComponentModel.DataAnnotations;
using game_x.domain.Enum.Rewards;

namespace game_x.domain.Entities.Rewards;

/// <summary>
/// Reward outcomes inside a reward pool.
/// Why needed: weighted random outcome engine.
/// </summary>
public sealed class RewardDefinition : BaseEntity<int>, IAuditable
{
    #region Identities
    public Guid PublicId { get; private set; } = Guid.CreateVersion7();

    /// <summary>
    /// Example:
    /// BALANCE_100
    /// TICKET_X1
    /// FREE_SPIN
    /// ROSE_X10
    /// NO_REWARD
    /// </summary>
    [MaxLength(128)]
    public string Code { get; private set; } = string.Empty;
    
    public int? CatalogItemId { get; private set; }
    #endregion

    #region Properties
    public RewardItemType Type { get; private set; }

    [MaxLength(256)]
    public string Title { get; private set; } = string.Empty;

    [MaxLength(4096)]
    public string? Description { get; private set; }

    /// <summary>Balance amount / item quantity / business reward amount.</summary>
    public decimal? Amount { get; private set; }

    [MaxLength(4096)]
    public string? Metadata { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime? DeletedAt { get; private set; }
    #endregion

    #region Relationships
    public CatalogItem? CatalogItem { get; private set; }
    
    private readonly List<RewardPoolItem> _rewardPoolItems = new();
    private readonly List<MissionReward> _missionRewards = new();
    private readonly List<UserReward> _userRewards = new();

    public IReadOnlyCollection<RewardPoolItem> RewardPoolItems => _rewardPoolItems;
    public IReadOnlyCollection<MissionReward> MissionRewards => _missionRewards;
    public IReadOnlyCollection<UserReward> UserRewards => _userRewards;
    #endregion

    #region Initializations
    private RewardDefinition() { }

    public static RewardDefinition Create(
        string code,
        RewardItemType type,
        string title,
        decimal? amount = null,
        string? description = null,
        string? metadata = null,
        int? catalogItemId = null)
    {
        return new()
        {
            Code = code,
            Type = type,
            Title = title,
            Description = description,
            Amount = amount,
            Metadata = metadata,
            IsActive = true
        };
    }
    #endregion

    #region Bahaviors
    public void AddCatalog(int id)
    {
        CatalogItemId = id;
    }
    
    public void AddCatalog(CatalogItem entity)
    {
        CatalogItem = entity;
    }
    
    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    public void SoftDelete() => DeletedAt = DateTime.UtcNow;
    #endregion
}