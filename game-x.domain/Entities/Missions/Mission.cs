using System.ComponentModel.DataAnnotations;
using game_x.domain.Enum.Missions;
using game_x.domain.ValueObjects.Missions;

namespace game_x.domain.Entities.Missions;

/// <summary>
/// Mission definitions.
/// Why needed: reusable mission engine.
/// </summary>
public sealed class Mission : BaseEntity<int>, IAuditable
{
    #region Identities
    public Guid PublicId { get; private set; } = Guid.CreateVersion7();
    
    public int? RewardConfigId { get; private set; }
    
    [MaxLength(128)]
    public string Code { get; private set; } = string.Empty;
    
    public DateTime? StartAt { get; private set; }

    public DateTime? EndAt { get; private set; }
    
    public DateTime? DeletedAt { get; private set; }
    #endregion

    #region Properties
    public MissionType Type { get; private set; }

    [MaxLength(256)]
    public string Title { get; private set; } = string.Empty;

    [MaxLength(4096)]
    public string? Description { get; private set; }
    
    public MissionResetType? ResetType { get; private set; }
    
    public bool IsActive { get; private set; } = true;
    
    public MissionConfigData ConfigData { get; private set; } = MissionConfigData.Default();
    #endregion

    #region Relationships
    public RewardPool? RewardPool { get; private set; }
    
    public ICollection<UserMission> UserMissions { get; private set; } = [];
    #endregion

    #region Initializations
    private Mission() { }

    public static Mission Create(
        string code,
        MissionType type,
        string title,
        MissionConfigData configData,
        int? rewardConfigId = null,
        string? description = null,
        MissionResetType? resetType = null,
        DateTime? startAt = null,
        DateTime? endAt = null)
    {
        return new()
        {
            Code = code,
            Type = type,
            Title = title,
            Description = description,
            ConfigData = configData,
            RewardConfigId = rewardConfigId,
            ResetType = resetType,
            StartAt = startAt,
            EndAt = endAt,
            IsActive = true
        };
    }
    #endregion

    #region Behaviors
    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    public void SoftDelete() => DeletedAt = DateTime.UtcNow;
    #endregion
}