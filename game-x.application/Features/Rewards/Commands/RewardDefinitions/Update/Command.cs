using game_x.domain.Enum.Rewards;
using Newtonsoft.Json;

namespace game_x.application.Features.Rewards.Commands.RewardDefinitions.Update;

public sealed record UpdateRewardDefinitionCommand : ICommand<Unit>
{
    [JsonIgnore]
    public Guid Id { get; init; }
    public string? Code { get; init; }
    public Guid? CatalogItemId { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public RewardItemType? Type { get; init; }
    public decimal? Amount { get; init; }
    public bool? IsActive { get; init; }
    public string? Metadata { get; init; }
}