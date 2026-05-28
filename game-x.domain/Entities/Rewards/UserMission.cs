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
    /// <summary>overall progress toward completion.</summary>
    public int Progress { get; private set; }

    /// <summary>Used for consecutive missions.</summary>
    public int Streak { get; private set; }
    
    public int CycleNumber { get; private set; }
    
    public UserMissionStatus Status { get; private set; }
    
    public DateTime? CompletedAt { get; private set; }

    public DateTime? ClaimedAt { get; private set; }

    public DateTime? ResetAt { get; private set; }
    
    public DateTime? LastProgressAt { get; private set; }
    #endregion
    
    #region Relationships
    public User? User { get; init; }
    
    public Mission? Mission { get; init; }
    
    private readonly List<UserMissionClaim> _userMissionClaim = new();
    public ICollection<UserMissionClaim> Claims => _userMissionClaim;
    #endregion

    #region Initializations
    public static UserMission Create(string userId, int missionId)
    {
        return new()
        {
            UserId = userId,
            MissionId = missionId,
            Progress = 0,
            Streak = 0,
            Status = UserMissionStatus.InProgress,
            CycleNumber = 1
        };
    }
    #endregion

    #region Behaviors
    public bool HasProgressToday(DateTime today) => LastProgressAt?.Date == today.Date;
    
    public bool InvalidTime(DateTime today) => LastProgressAt?.Date != null && today < LastProgressAt?.Date;

    public bool IsMissedRequiredDay(DateTime today)
        => LastProgressAt.HasValue &&
           LastProgressAt.Value.Date < today.AddDays(-1).Date;

    public void OnAddProgress(int value, DateTime at, bool consecutive)
    {
        if (Status == UserMissionStatus.Completed || Status == UserMissionStatus.Claimed)
            return;

        Progress += value;

        if (consecutive) Streak++;
        else Streak = 1;

        LastProgressAt = at;
    }

    public void OnResetProgress()
    {
        Progress = 0;
        Streak = 0;
        Status = UserMissionStatus.InProgress;
        CompletedAt = null;
        ClaimedAt = null;
        ResetAt = DateTime.UtcNow;
        CycleNumber++;
    }
    
    public void OnComplete()
    {
        if (Status == UserMissionStatus.Completed) return;

        Status = UserMissionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void OnClaim()
    {
        if (Status != UserMissionStatus.Completed) return;
        Status = UserMissionStatus.Claimed;
        ClaimedAt = DateTime.UtcNow;
    }
    #endregion
}