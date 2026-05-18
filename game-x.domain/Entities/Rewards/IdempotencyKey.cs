using System.ComponentModel.DataAnnotations;
using game_x.domain.Enum.Rewards;

namespace game_x.domain.Entities.Rewards;

/// <summary>
/// Duplicate request protection.
/// Why needed: money/reward safety.
/// </summary>
public sealed class IdempotencyKey : BaseEntity<int>
{
    #region Identities
    [MaxLength(256)]
    public string Key { get; private set; } = string.Empty;

    [MaxLength(36)]
    public string UserId { get; private set; } = string.Empty;
    #endregion

    #region Properties
    public IdempotencyActionType ActionType { get; private set; }

    [MaxLength(4096)]
    public string? ResponseMetadata { get; private set; }
    
    public DateTime? ExpiredAt { get; private set; }
    #endregion

    #region Relationships
    public User? User { get; init; }
    #endregion

    #region Initializations
    private IdempotencyKey() { }

    public static IdempotencyKey Create(
        string key,
        string userId,
        IdempotencyActionType actionType,
        string? responseMetadata = null,
        DateTime? expiredAt = null)
    {
        return new()
        {
            Key = key,
            UserId = userId,
            ActionType = actionType,
            ResponseMetadata = responseMetadata,
            ExpiredAt = expiredAt
        };
    }
    #endregion

    #region Behaviors
    public bool IsExpired()
    {
        return ExpiredAt.HasValue && ExpiredAt.Value <= DateTime.UtcNow;
    }

    public void SetResponse(string responseMetadata)
    {
        ResponseMetadata = responseMetadata;
    }
    #endregion
}