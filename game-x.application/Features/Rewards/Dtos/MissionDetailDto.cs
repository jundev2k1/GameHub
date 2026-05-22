using game_x.domain.Enum.Rewards;
using game_x.domain.ValueObjects.Missions;

namespace game_x.application.Features.Rewards.Dtos;

public sealed class MissionDetailDto
{
    public Guid Id { get; set; }
    
    public string Code { get; set; } = string.Empty;
    
    public MissionType Type { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public MissionResetType? ResetType { get; set; }
    
    public bool IsActive { get; set; }
    
    public UserEventType[] TriggerEvents { get; set; } = [];
    
    public MissionConfigData ConfigData { get; set; } = MissionConfigData.Default();
    
    public DateTime? StartAt { get; set; }
    
    public DateTime? EndAt { get; set; }

    public IReadOnlyCollection<MissionRewardDetailDto> MissionRewards { get; set; } = [];
}