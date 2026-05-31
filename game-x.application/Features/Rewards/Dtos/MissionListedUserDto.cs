using game_x.domain.Enum.Rewards;

namespace game_x.application.Features.Rewards.Dtos;

public sealed class MissionListedUserDto
{
    public Guid Id { get; set; }
    
    public string Code { get; set; } = string.Empty;
    
    public MissionType Type { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
}