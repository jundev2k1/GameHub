using game_x.domain.Enum.Rewards;
using game_x.domain.ValueObjects.Missions;

namespace game_x.application.Features.Rewards.Dtos;

public sealed class MissionUserDto
{
    public Guid Id { get; set; }
    
    public string Code { get; set; } = string.Empty;
    
    public MissionType Type { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public MissionResetType? ResetType { get; set; }
    
    public MissionConfigData ConfigData { get; set; } = MissionConfigData.Default();
    
    public IReadOnlyCollection<MissionRewardUserDto> MissionRewards { get; set; } = [];
  
    /// <summary>overall progress toward completion.</summary>
    public int? Progress { get; set; }

    /// <summary>Used for consecutive missions.</summary>
    public int? Streak { get; set; }
    
    public UserMissionStatus? Status { get; set; }
    
    public DateTime? LastProgressAt { get; set; }
}