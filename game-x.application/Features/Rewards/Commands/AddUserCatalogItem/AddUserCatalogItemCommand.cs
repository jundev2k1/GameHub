using System.Text.Json.Serialization;

namespace game_x.application.Features.Rewards.Commands.AddUserCatalogItem;

public sealed record AddUserCatalogItemCommand: ICommand<Unit>
{
    [JsonIgnore]
    public string UserId { get; init; } = String.Empty;
    public string Code { get; init; } = String.Empty;
    public int Quantity { get; init; }
}