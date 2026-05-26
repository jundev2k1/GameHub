using game_x.domain.Enum.Rewards;

namespace game_x.application.Features.Rewards.Dtos;

public sealed class MissionUserDto
{
    public Guid Id { get; set; }
    
    public string Code { get; set; } = string.Empty;
    
    public MissionType Type { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public MissionResetType? ResetType { get; set; }
    
    public IReadOnlyCollection<MissionRewardUserDto> MissionRewards { get; set; } = [];
    
    public DateTime? LastProgressAt { get; set; }
}