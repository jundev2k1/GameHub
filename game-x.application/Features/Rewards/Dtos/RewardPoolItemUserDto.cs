using game_x.domain.Enum.Rewards;

namespace game_x.application.Features.Rewards.Dtos;

public sealed class RewardPoolItemUserDto
{
    public Guid Id { get; set; }
   
    public Guid RewardDefinitionId { get; set; }
    
    public string RewardDefinitionCode { get; set; } = string.Empty;
    
    public decimal? Amount { get; set; }
    
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public Guid? ItemId { get; set; }
    
    public string? ItemName { get; set; }
    
    public RewardItemType? RewardType { get; set; }
    
    public CatalogItemIconType? ItemIconType { get; set; }
    
    public string? ItemIconValue { get; set; }
    
    public string? ItemIconUrl { get; set; }
}