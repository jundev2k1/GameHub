using game_x.domain.Enum.Rewards;

namespace game_x.application.Features.Rewards.Dtos;

public sealed record RewardDefinitionDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public RewardItemType Type { get; set; }
    public decimal? Amount { get; set; }
    public string? Metadata { get; set; }
    public bool IsActive { get; set; }
    public Guid? ItemId { get; set; }
    public string? ItemName { get; set; }
    public CatalogItemIconType? ItemIconType { get; set; }
    public string? ItemIconValue { get; set; }
    public string? ItemIconUrl { get; set; }
}