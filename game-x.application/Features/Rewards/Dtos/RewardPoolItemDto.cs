using game_x.domain.Enum.Rewards;

namespace game_x.application.Features.Rewards.Dtos;

public sealed class RewardPoolItemDto
{
    public Guid Id { get; set; }
    
    public int Weight { get; set; }
    
    public int SortOrder { get; set; }
    
    public bool IsActive { get; private set; }
    
    public DateTime? StartAt { get; private set; }

    public DateTime? EndAt { get; private set; }
   
    public Guid RewardDefinitionId { get; set; }
    
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