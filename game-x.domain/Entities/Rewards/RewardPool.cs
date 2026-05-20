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
    
    public UserEventType[] TriggerEvents { get; private set; } = [];
    
    public RewardPoolConfigData? Config { get; private set; }
    
    public DateTime? DeletedAt { get; private set; }
    
    public DateTime? StartAt { get; private set; }
    
    public DateTime? EndAt { get; private set; }
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
        UserEventType[] triggerEvents,
        string code,
        string title,
        string? description = null,
        RewardPoolConfigData? config = null)
    {
        return new()
        {
            Type = type,
            Code = code,
            TriggerEvents = triggerEvents,
            Title = title,
            Description = description,
            Config = config
        };
    }
    #endregion

    #region Behaviors

    public void OnUpdate(
        string? code = null,
        string? title = null,
        string? description = null,
        RewardPoolType? type = null,
        bool? isActive = null,
        int? sortOrder = null,
        DateTime? startAt = null,
        DateTime? endAt = null,
        RewardPoolConfigData? config = null)
    {
        Code = code ?? Code;
        Title = title ?? Title;
        Type = type ?? Type;
        Description = description ?? Description;
        IsActive = isActive ?? IsActive;
        SortOrder = sortOrder ?? SortOrder;
        StartAt = startAt ?? StartAt;
        EndAt = endAt ?? EndAt;
        Config = config ?? Config;
    }

    public void SoftDelete() => DeletedAt = DateTime.UtcNow;
    #endregion
}