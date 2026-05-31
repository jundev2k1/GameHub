using System.Text.Json.Serialization;

namespace game_x.application.Features.Rewards.Commands.CatalogItems.AddToUserInventory;

public sealed record AddItemToUserInventoryCommand: ICommand<Unit>
{
    [JsonIgnore]
    public string UserId { get; init; } = String.Empty;
    public string Code { get; init; } = String.Empty;
    public int Quantity { get; init; }
}