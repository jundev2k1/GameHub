using System.ComponentModel.DataAnnotations;
using game_x.domain.Enum.Rewards;

namespace game_x.domain.Entities.Rewards;

public sealed class UserMissionClaim : BaseEntity<int>, IAuditable
{
    #region Identities

    public Guid PublicId { get; private set; } = Guid.CreateVersion7();

    [MaxLength(36)]
    public string UserId { get; private set; } = string.Empty;

    public int UserMissionId { get; private set; }

    public int MissionRewardId { get; private set; }

    public int? ExecutionId { get; private set; }

    #endregion

    #region Properties

    public UserMissionClaimStatus Status { get; private set; }

    public DateTime AvailableAt { get; private set; }

    public DateTime? ClaimedAt { get; private set; }

    public DateTime? ExpiredAt { get; private set; }

    #endregion

    #region Relationships

    public User? User { get; init; }

    public UserMission? UserMission { get; init; }

    public MissionReward? MissionReward { get; init; }

    public Execution? Execution { get; private set; }

    #endregion

    #region Initializations

    private UserMissionClaim() { }

    public static UserMissionClaim Create(
        string userId,
        UserMission userMission,
        int missionRewardId,
        DateTime? availableAt = null)
    {
        return new()
        {
            UserId = userId,
            UserMission = userMission,
            MissionRewardId = missionRewardId,
            Status = UserMissionClaimStatus.Available,
            AvailableAt = availableAt ?? DateTime.UtcNow
        };
    }

    #endregion

    #region Behaviors

    public void Claim(Execution execution)
    {
        if (Status != UserMissionClaimStatus.Available)
            throw new InvalidOperationException("Reward is not claimable.");

        Status = UserMissionClaimStatus.Claimed;
        Execution = execution;
        ClaimedAt = DateTime.UtcNow;
    }

    public void Expire()
    {
        if (Status == UserMissionClaimStatus.Claimed)
            throw new InvalidOperationException("Claim already completed.");

        Status = UserMissionClaimStatus.Expired;
        ExpiredAt = DateTime.UtcNow;
    }

    #endregion
}