using System.ComponentModel.DataAnnotations;
using game_x.domain.Enum.Rewards;

namespace game_x.domain.Entities.Rewards;

/// <summary>
/// Reward grant ledger.
/// Why needed: immutable reward audit history.
/// </summary>
public sealed class UserReward : BaseEntity<int>, IAuditable
{
    #region Identities

    public Guid PublicId { get; private set; } = Guid.CreateVersion7();

    [MaxLength(36)]
    public string UserId { get; private set; } = string.Empty;

    public int ExecutionId { get; private set; }

    /// <summary>
    /// Source reward definition.
    /// Mission reward / reward pool reward / promo reward.
    /// </summary>
    public int? RewardDefinitionId { get; private set; }

    /// <summary>Source pool item if reward comes from roulette/gacha.</summary>
    public int? RewardPoolItemId { get; private set; }

    /// <summary>Related balance transaction if applicable.</summary>
    public int? TransactionId { get; private set; }

    /// <summary>Catalog item if inventory-based reward.</summary>
    public int? CatalogItemId { get; private set; }

    #endregion

    #region Properties

    /// <summary>Immutable reward type snapshot.</summary>
    public RewardItemType RewardType { get; private set; }

    /// <summary>Immutable granted amount snapshot.</summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// Optional display title snapshot.
    /// Protect against later admin edits.
    /// </summary>
    [MaxLength(256)]
    public string? Title { get; private set; }

    public UserRewardStatus Status { get; private set; }

    [MaxLength(4096)]
    public string? Metadata { get; private set; }

    public DateTime GrantedAt { get; private set; }

    public DateTime? ClaimedAt { get; private set; }

    public DateTime? ExpiredAt { get; private set; }

    public DateTime? RevokedAt { get; private set; }

    #endregion

    #region Relationships

    public User? User { get; init; }

    public Execution? Execution { get; init; }

    public RewardDefinition? RewardDefinition { get; init; }

    public RewardPoolItem? RewardPoolItem { get; init; }

    public Transaction? Transaction { get; init; }

    public CatalogItem? CatalogItem { get; init; }

    #endregion

    #region Initialization

    private UserReward() { }

    public static UserReward Create(
        string userId,
        Execution execution,
        RewardItemType rewardType,
        decimal amount,
        int? rewardDefinitionId = null,
        int? rewardPoolItemId = null,
        int? catalogItemId = null,
        int? transactionId = null,
        string? title = null,
        string? metadata = null,
        DateTime? expiredAt = null)
    {
        return new()
        {
            UserId = userId,
            Execution = execution,
            RewardDefinitionId = rewardDefinitionId,
            RewardPoolItemId = rewardPoolItemId,
            CatalogItemId = catalogItemId,
            TransactionId = transactionId,
            RewardType = rewardType,
            Amount = amount,
            Title = title,
            Metadata = metadata,
            GrantedAt = DateTime.UtcNow,
            ExpiredAt = expiredAt,
            Status = UserRewardStatus.Granted
        };
    }

    #endregion

    #region Behaviors

    public void Claim()
    {
        if (Status != UserRewardStatus.Granted)
            throw new InvalidOperationException();

        Status = UserRewardStatus.Claimed;
        ClaimedAt = DateTime.UtcNow;
    }

    public void Expire()
    {
        if (Status == UserRewardStatus.Claimed)
            throw new InvalidOperationException();

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