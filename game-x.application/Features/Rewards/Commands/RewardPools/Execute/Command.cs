using game_x.domain.Enum.Rewards;

namespace game_x.application.Features.Rewards.Commands.RewardPools.Execute;

public sealed record RewardPoolExecuteCommand(Guid RewardPoolPublicId) : ICommand<RewardPoolExecuteResponse>;

public sealed class RewardPoolExecuteResponse
{
    public Guid ExecutionId { get; init; }

    public Guid RewardId { get; init; }
    
    public string RewardCode { get; init; } = string.Empty;

    public string RewardTitle { get; init; } = string.Empty;

    public RewardItemType RewardType { get; init; }

    public decimal? Amount { get; init; }
}