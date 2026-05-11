using System.ComponentModel.DataAnnotations;
using game_x.domain.Enum.Missions;

namespace game_x.domain.Entities.Missions;

/// <summary>
/// Reward grant ledger.
/// Why needed: immutable reward audit history.
/// </summary>
public sealed class UserReward : BaseEntity<int>, IAuditable
{
    #region Indetities
    public Guid PublicId { get; private set; } = Guid.CreateVersion7();
    
    [MaxLength(36)]
    public string UserId { get; private set; } = string.Empty;
    
    public int ExecutionId { get; private set; }
    
    public int RewardPoolId { get; private set; }
    
    public int RewardItemId { get; private set; }
    
    public int? TransactionId { get; private set; }
    
    public int? InventoryItemDefinitionId { get; private set; }
    #endregion

    #region Properties
    public RewardItemType RewardType { get; private set; }
    
    public decimal Amount { get; private set; }
    
    public UserRewardStatus Status { get; private set; } = UserRewardStatus.Granted;

    [MaxLength(4096)]
    public string? Metadata { get; private set; }
    
    public DateTime GrantedAt { get; private set; }

    public DateTime? ExpiredAt { get; private set; }
    
    public DateTime? RevokedAt { get; private set; }
    #endregion

    #region Relationships
    public User? User { get; private set; }
    
    public Execution? Execution { get; private set; }

    public RewardPool? RewardPool { get; private set; }

    public RewardItem? RewardItem { get; private set; }

    public Transaction? Transaction { get; private set; }
    
    public InventoryItemDefinition? InventoryItemDefinition { get; private set; }
    #endregion

    #region Initializations
    private UserReward() { }

    public static UserReward Create(
        string userId,
        int executionId,
        int rewardPoolId,
        int rewardItemId,
        RewardItemType rewardType,
        decimal amount,
        int? inventoryItemDefinitionId = null,
        int? transactionId = null,
        string? metadata = null,
        DateTime? expiredAt = null)
    {
        return new()
        {
            UserId = userId,
            ExecutionId = executionId,
            RewardPoolId = rewardPoolId,
            RewardItemId = rewardItemId,
            RewardType = rewardType,
            Amount = amount,
            InventoryItemDefinitionId = inventoryItemDefinitionId,
            TransactionId = transactionId,
            Metadata = metadata,
            GrantedAt = DateTime.UtcNow,
            ExpiredAt = expiredAt,
            Status = UserRewardStatus.Granted
        };
    }
    #endregion
    
    #region Behaviors
    public void Expire()
    {
        Status = UserRewardStatus.Expired;
        ExpiredAt = DateTime.UtcNow;
    }

    public void Revoke()
    {
        Status = UserRewardStatus.Revoked;
        RevokedAt = DateTime.UtcNow;
    }
    #endregion
}