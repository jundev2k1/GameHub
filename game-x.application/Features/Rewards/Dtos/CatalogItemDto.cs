using game_x.domain.Enum.Rewards;

namespace game_x.application.Features.Rewards.Dtos;

public sealed record CatalogItemDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CatalogItemCategory Category { get; set; }
    public decimal? MonetaryValue { get; set; }
    public CatalogItemIconType IconType { get; set; }
    public string? IconValue { get; set; }
    public bool IsActive { get; set; }
    public string? IconUrl { get; set; }
}