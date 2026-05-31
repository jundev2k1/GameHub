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
    
    public IdempotencyStatus Status { get; private set; }
    [MaxLength(4096)]
    public string? ResponsePayload { get; private set; }
    
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
        string? responsePayload = null,
        DateTime? expiredAt = null)
    {
        return new()
        {
            Key = key,
            UserId = userId,
            ActionType = actionType,
            Status = IdempotencyStatus.Processing,
            ResponsePayload = responsePayload,
            ExpiredAt = expiredAt
        };
    }
    #endregion

    #region Behaviors
    
    public bool IsExpired()
    {
        return ExpiredAt.HasValue && ExpiredAt.Value <= DateTime.UtcNow;
    }

    public void SetResponse(string responsePayload)
    {
        ResponsePayload = responsePayload;
    }
    
    public void MarkCompleted()
    {
        Status = IdempotencyStatus.Completed;
    }
    
    public void MarkFailed()
    {
        Status = IdempotencyStatus.Failed;
    }
    #endregion
}