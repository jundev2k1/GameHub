using System.ComponentModel.DataAnnotations;
using game_x.domain.Enum.Rewards;
using game_x.domain.ValueObjects.Missions;

namespace game_x.domain.Entities.Rewards;

/// <summary>
/// Reward engine configuration. Defines playable reward systems.
/// Why needed: reusable configurable reward engine.
/// </summary>
public sealed class RewardPool : BaseEntity<int>, IAuditable
{
    #region Identities
    public Guid PublicId { get; private set; } = Guid.CreateVersion7();
    
    /// <summary>Example: MAIN_ROULETTE, SUMMER_EVENT_2026, VIP_GACHA, NEW_USER_WHEEL.</summary>
    [MaxLength(128)]
    public string Code { get; private set; } = string.Empty;
    #endregion
    
    #region Properties
    public RewardPoolType Type { get; private set; } = RewardPoolType.Roulette;
      
    [MaxLength(2048)]
    public string Title { get; private set; } = string.Empty;
    
    [MaxLength(4096)]
    public string? Description { get; private set; }
    
    public bool IsActive { get; private set; } = true;
    
    public int SortOrder { get; private set; }
    
    public RewardPoolConfigData? Config { get; private set; }
    
    public DateTime? DeletedAt { get; private set; }
    
    public DateTime? StartAt { get; init; }
    
    public DateTime? EndAt { get; init; }
    #endregion
    
    #region Relationships
    private readonly List<RewardPoolItem> _rewardPoolItems = new();
    private readonly List<Execution> _execution = new();
    
    public IReadOnlyCollection<RewardPoolItem> RewardPoolItems => _rewardPoolItems;
    
    public IReadOnlyCollection<Execution> Executions => _execution;
    #endregion

    #region Initializations
    private RewardPool() { }

    public static RewardPool Create(
        RewardPoolType type,
        string code,
        string title,
        string? description = null,
        RewardPoolConfigData? config = null)
    {
        return new()
        {
            Type = type,
            Code = code,
            Title = title,
            Description = description,
            Config = config
        };
    }
    #endregion

    #region Behaviors
    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    public void SoftDelete() => DeletedAt = DateTime.UtcNow;
    #endregion
}