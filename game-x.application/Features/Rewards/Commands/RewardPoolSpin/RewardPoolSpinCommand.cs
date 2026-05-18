using game_x.domain.Enum.Rewards;

namespace game_x.application.Features.Rewards.Commands.RewardPoolSpin;

public sealed record RewardPoolSpinCommand(Guid RewardPoolPublicId ) : ICommand<SpinRewardResponse>;

public sealed class SpinRewardResponse
{
    public Guid ExecutionId { get; init; }

    public string RewardCode { get; init; } = string.Empty;

    public string RewardTitle { get; init; } = string.Empty;

    public RewardItemType RewardType { get; init; }

    public decimal? Amount { get; init; }
}