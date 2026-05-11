using System.ComponentModel.DataAnnotations;
using game_x.domain.Enum.Missions;

namespace game_x.domain.Entities.Missions;

/// <summary>
/// Universal operation audit log.
/// Why needed: audit/debug/replay trace.
/// </summary>
public sealed class Execution : BaseEntity<int>
{
    #region Identities
    public Guid PublicId { get; private set; } = Guid.CreateVersion7();

    [MaxLength(36)]
    public string UserId { get; private set; } = string.Empty;

    public int? RewardPoolId { get; private set; }

    public int? MissionId { get; private set; }

    [MaxLength(256)]
    public string? IdempotencyKey { get; private set; }
    #endregion

    #region Properties
    public ExecutionType Type { get; private set; }

    public ExecutionStatus Status { get; private set; }

    [MaxLength(4096)]
    public string? ResultMetadata { get; private set; }

    [MaxLength(2048)]
    public string? ErrorMessage { get; private set; }

    #endregion

    #region Relationships
    public User? User { get; private set; }

    public RewardPool? RewardPool { get; private set; }

    public Mission? Mission { get; private set; }

    public ICollection<UserReward> UserRewards { get; private set; } = [];
    #endregion

    #region Initializations
    private Execution() { }

    public static Execution Create(
        string userId,
        ExecutionType type,
        ExecutionStatus status = ExecutionStatus.Pending,
        int? rewardPoolId = null,
        int? missionId = null,
        string? idempotencyKey = null)
    {
        return new()
        {
            UserId = userId,
            Type = type,
            Status = status,
            RewardPoolId = rewardPoolId,
            MissionId = missionId,
            IdempotencyKey = idempotencyKey
        };
    }
    #endregion

    #region Behaviors
    public void MarkSuccess(string? resultMetadata = null)
    {
        Status = ExecutionStatus.Success;
        ResultMetadata = resultMetadata;
        ErrorMessage = null;
    }

    public void MarkFailed(string? errorMessage = null)
    {
        Status = ExecutionStatus.Failed;
        ErrorMessage = errorMessage;
    }
    #endregion
}