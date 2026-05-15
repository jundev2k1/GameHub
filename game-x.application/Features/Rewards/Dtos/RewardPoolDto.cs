using game_x.domain.Enum.Rewards;
using game_x.domain.ValueObjects.Missions;

namespace game_x.application.Features.Rewards.Dtos;

public sealed class RewardPoolDto
{
    public Guid Id { get; set; }
    
    public string Code { get; private set; } = string.Empty;
    
    public RewardPoolType Type { get; set; }
    
    public string Title { get; private set; } = string.Empty;
    
    public string? Description { get; private set; }
    
    public bool IsActive { get; private set; }
    
    public RewardPoolConfigData Config { get; private set; } = RewardPoolConfigData.Default();
    
    public DateTime? StartAt { get; private set; }

    public DateTime? EndAt { get; private set; }
}