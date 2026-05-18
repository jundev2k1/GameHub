using System.ComponentModel.DataAnnotations;
using game_x.domain.Enum.Rewards;

namespace game_x.domain.Entities.Rewards;

public sealed class UserMission : BaseEntity<int>
{
    #region Indenties
    [MaxLength(36)]
    public string UserId { get; private set; } = string.Empty;
    
    public int MissionId { get; private set; }
    #endregion
    
    #region Properties
    public int Progress { get; private set; }

    public int Streak { get; private set; }
    
    public UserMissionStatus Status { get; private set; }
    
    public DateTime? CompletedAt { get; private set; }

    public DateTime? ClaimedAt { get; private set; }

    public DateTime? ResetAt { get; private set; }
    #endregion
    
    #region Relationships
    public User? User { get; init; }
    
    public Mission? Mission { get; init; }
    
    private readonly List<UserMissionClaim> _userMissionClaim = new();
    public IReadOnlyCollection<UserMissionClaim> Claims => _userMissionClaim;
    #endregion

    #region Initializations
    private UserMission() { }

    public static UserMission Create(string userId, int missionId)
    {
        return new()
        {
            UserId = userId,
            MissionId = missionId,
            Progress = 0,
            Streak = 0,
            Status = UserMissionStatus.InProgress
        };
    }
    #endregion

    #region Behaviors
    public void AddProgress(int amount = 1)
    {
        if (Status.Equals(UserMissionStatus.Completed)) return;
        Progress += amount;
    }

    public void Complete()
    {
        Status = UserMissionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Claim()
    {
        Status = UserMissionStatus.Claimed;
        ClaimedAt = DateTime.UtcNow;
    }

    public void Reset()
    {
        Progress = 0;
        Status = UserMissionStatus.InProgress;
        CompletedAt = null;
        ClaimedAt = null;
        ResetAt = DateTime.UtcNow;
    }
    #endregion
}