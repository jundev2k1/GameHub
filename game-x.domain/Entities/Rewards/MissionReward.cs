namespace game_x.domain.Entities.Rewards;

/// <summary>
/// Mission definitions.
/// Why needed: reusable mission engine.
/// </summary>
public sealed class MissionReward : BaseEntity<int>, IAuditable
{
    #region Identities
    public Guid PublicId { get; private set; } = Guid.CreateVersion7();

    public int MissionId { get; private set; }

    public int RewardDefinitionId { get; private set; }
    #endregion
    
    #region Properties
    /// <summary>
    /// Milestone number in mission progression.
    ///
    /// Examples:
    /// Daily login:
    /// Day 1 = 1
    /// Day 7 = 7
    ///
    /// Deposit ladder:
    /// Tier 1 = 1
    /// Tier 2 = 2
    /// </summary>
    public int Sequence { get; private set; }
    
    public int SortOrder { get; private set; }

    /// <summary>
    /// Required progress to unlock this reward. Trigger progress
    ///
    /// Examples:
    /// Login mission: 1,2,3,4...
    /// Deposit mission: 100,500,1000
    /// Share mission: 1 click
    /// </summary>
    public decimal RequiredProgress { get; private set; }

    /// <summary>
    /// Allow manual claiming.
    /// false = auto grant.
    /// </summary>
    public bool IsClaimable { get; private set; } = true;

    public bool IsActive { get; private set; } = true;

    public DateTime? StartAt { get; private set; }

    public DateTime? EndAt { get; private set; }

    public DateTime? DeletedAt { get; private set; }
    #endregion

    #region Relationships
    public Mission? Mission { get; init; }

    public RewardDefinition? RewardDefinition { get; init; }
    
    private readonly List<MissionReward> _missionRewards = new();
    
    private readonly List<UserMissionClaim> _userMissionClaim = new();
    
    public ICollection<MissionReward> MissionRewards => _missionRewards;
    
    public ICollection<UserMissionClaim> UserMissionClaims => _userMissionClaim;
    #endregion

    #region Initializations
    private MissionReward() { }

    public static MissionReward Create(
        int missionId,
        int rewardDefinitionId,
        int sequence,
        int sortOrder,
        decimal requiredProgress,
        bool? isClaimable = null,
        DateTime? startAt = null,
        DateTime? endAt = null)
    {
        if (sequence <= 0)
            throw new ArgumentOutOfRangeException(nameof(sequence));

        if (requiredProgress <= 0)
            throw new ArgumentOutOfRangeException(nameof(requiredProgress));

        return new()
        {
            MissionId = missionId,
            RewardDefinitionId = rewardDefinitionId,
            Sequence = sequence,
            SortOrder = sortOrder,
            RequiredProgress = requiredProgress,
            IsClaimable = isClaimable ?? true,
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