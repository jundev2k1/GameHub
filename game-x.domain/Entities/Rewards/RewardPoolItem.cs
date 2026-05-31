namespace game_x.domain.Entities.Rewards;

/// <summary>
/// Reward outcomes inside a reward pool.
/// Why needed: weighted random outcome engine.
/// </summary>
public sealed class RewardPoolItem : BaseEntity<int>, IAuditable
{
    #region Identities
    public Guid PublicId { get; private set; } = Guid.CreateVersion7();

    public int RewardPoolId { get; private set; }

    public int RewardDefinitionId { get; private set; }
    #endregion

    #region Properties
    /// <summary>
    /// The number of tickets corresponds to the weight in the reward pool.
    /// If weight = 0, no tickets are assigned, meaning there is no chance of winning</summary>
    public int Weight { get; private set; }

    public int SortOrder { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime? StartAt { get; private set; }

    public DateTime? EndAt { get; private set; }
    #endregion

    #region Relationships
    public RewardPool? RewardPool { get; init; }

    public RewardDefinition? RewardDefinition { get; init; }
    
    private readonly List<UserReward> _userRewards = new();
    public ICollection<UserReward> UserRewards => _userRewards;
    #endregion

    #region Initializations
    private RewardPoolItem() { }

    public static RewardPoolItem Create(
        int rewardPoolId,
        int rewardDefinitionId,
        int weight,
        int sortOrder = 0,
        DateTime? startAt = null,
        DateTime? endAt = null)
    {
        if(weight < 1) throw new ArgumentException("Weight must be greater than zero.");
        
        return new()
        {
            RewardPoolId = rewardPoolId,
            RewardDefinitionId = rewardDefinitionId,
            Weight = weight,
            SortOrder = sortOrder,
            StartAt = startAt,
            EndAt = endAt,
            IsActive = true
        };
    }
    #endregion

    #region Behaviors
    public void OnUpdate(
        int? rewardDefinitionId = null,
        int? weight = null,
        bool? isActive = null,
        int? sortOrder = null,
        DateTime? startAt = null,
        DateTime? endAt = null)
    {
        RewardDefinitionId = rewardDefinitionId ?? RewardDefinitionId;
        Weight = weight ?? Weight;
        IsActive = isActive ?? IsActive;
        SortOrder = sortOrder ?? SortOrder;
        StartAt = startAt ?? StartAt;
        EndAt = endAt ?? EndAt;
    }
    #endregion
}