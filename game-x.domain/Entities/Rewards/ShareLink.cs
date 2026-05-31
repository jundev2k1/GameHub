using System.ComponentModel.DataAnnotations;
using game_x.domain.Enum.Rewards;

namespace game_x.domain.Entities.Rewards;

/// <summary>
/// Social mission shares token tracking.
/// Why needed: validate social share completion.
/// </summary>
public sealed class ShareLink : BaseEntity<int>
{
    #region Identities

    public Guid PublicId { get; private set; } = Guid.CreateVersion7();

    [MaxLength(36)]
    public string UserId { get; private set; } = string.Empty;

    public int MissionId { get; private set; }

    /// <summary>
    /// Public share token.
    /// Example: abc123xyz
    /// </summary>
    [MaxLength(128)]
    public string Code { get; private set; } = string.Empty;

    #endregion

    #region Properties

    public int ClickCount { get; private set; }

    public ShareLinkStatus Status { get; private set; } = ShareLinkStatus.Active;
    
    public DateTime? CompletedAt { get; private set; }

    public DateTime? ExpiredAt { get; private set; }
    #endregion

    #region Relationships

    public User? User { get; init; }

    public Mission? Mission { get; init; }

    #endregion

    #region Initializations
    private ShareLink() { }

    public static ShareLink Create(
        string userId,
        int missionId,
        string code,
        DateTime? expiredAt = null)
    {
        return new()
        {
            UserId = userId,
            MissionId = missionId,
            Code = code,
            ClickCount = 0,
            ExpiredAt = expiredAt,
            Status = ShareLinkStatus.Active
        };
    }
    #endregion

    #region Behaviors
    public void IncreaseClick()
    {
        if (IsExpired())
            throw new InvalidOperationException("Share link expired.");

        if (Status != ShareLinkStatus.Active)
            throw new InvalidOperationException("Share link inactive.");

        ClickCount++;
    }

    public void Complete()
    {
        Status = ShareLinkStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Expire()
    {
        Status = ShareLinkStatus.Expired;
    }

    public bool IsExpired()
    {
        return ExpiredAt.HasValue && ExpiredAt.Value <= DateTime.UtcNow;
    }
    #endregion
}