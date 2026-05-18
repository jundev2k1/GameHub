using game_x.domain.Enum.Rewards;
using game_x.domain.ValueObjects.Missions;

namespace game_x.application.Features.Rewards.Dtos;

public sealed class RewardPoolDto
{
    public Guid Id { get; set; }
    
    public string Code { get; set; } = string.Empty;
    
    public RewardPoolType Type { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
    
    public RewardPoolConfigData Config { get; set; } = RewardPoolConfigData.Default();
    
    public DateTime? StartAt { get; set; }

    public DateTime? EndAt { get; set; }
}