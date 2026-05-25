using game_x.domain.Enum.Rewards;
using game_x.domain.ValueObjects.Missions;

namespace game_x.application.Features.Rewards.Dtos;

public sealed class UserMissionDto
{
    public Guid Id { get; set; }
    
    public string Code { get; set; } = string.Empty;
    
    public MissionType Type { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public MissionResetType? ResetType { get; set; }
    
    public bool IsActive { get; set; }
    
    public MissionConfigData ConfigData { get; set; } = MissionConfigData.Default();
    
    public IReadOnlyCollection<UserMissionRewardDto> MissionRewards { get; set; } = [];
    
    public DateTime? LastProgressAt { get; set; }
}