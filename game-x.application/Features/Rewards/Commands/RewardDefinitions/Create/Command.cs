using game_x.domain.Enum.Rewards;

namespace game_x.application.Features.Rewards.Commands.RewardDefinitions.Create;

public sealed record CreateRewardDefinitionCommand: ICommand<Unit>
{
    public Guid? CatalogItemId { get; init; }
    public string Code { get; init; } = String.Empty;
    public string Title { get; init; } = String.Empty;
    public RewardItemType Type { get; init; }
    public string? Description { get; init; }
    public decimal? Amount { get; init; }
    public string? Metadata { get; init; }
}