using game_x.domain.Enum.Rewards;

namespace game_x.application.Features.Rewards.Dtos;

public sealed record UserInventoryDto
{
    public Guid CatalogItemId { get; set; }
    public int Quantity { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public CatalogItemCategory Category { get; set; }
    public CatalogItemIconType IconType { get; set; }
    public string? IconValue { get; set; }
    public string? IconUrl { get; set; }
}