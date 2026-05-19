using game_x.domain.Enum.Rewards;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace game_x.application.Features.Rewards.Commands.CatalogItems.Update;

public sealed record UpdateCatalogItemCommand : ICommand<Unit>
{
    [JsonIgnore]
    public Guid Id { get; init; }
    public string? Code { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public CatalogItemCategory? Category { get; init; }
    public decimal? MonetaryValue { get; init; }
    public CatalogItemIconType? IconType { get; init; }
    public string? IconValue { get; init; }
    public bool? IsActive { get; init; }
    public int? SortOrder { get; init; }
    public IFormFile? Icon { get; init; }
}