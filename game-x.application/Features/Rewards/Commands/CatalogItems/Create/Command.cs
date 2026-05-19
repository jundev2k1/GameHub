using game_x.domain.Enum.Rewards;
using Microsoft.AspNetCore.Http;

namespace game_x.application.Features.Rewards.Commands.CatalogItems.Create;

public sealed record CreateCatalogItemCommand: ICommand<Unit>
{
    public string Code { get; init; } = String.Empty;
    public string Name { get; init; } = String.Empty;
    public CatalogItemCategory Category { get; init; }
    public int SortOrder { get; init; }
    public string? Description { get; init; }
    public decimal? MonetaryValue { get; init; }
    public CatalogItemIconType IconType { get; init; }
    public string? IconValue { get; init; }
    public IFormFile? Icon { get; init; }
}