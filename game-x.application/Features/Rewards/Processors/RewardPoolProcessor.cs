using game_x.domain.Entities.Rewards;

namespace game_x.application.Features.Rewards.Processors;

public interface IRewardPoolProcessor
{
    Task ProcessAsync(RewardPool rewardPool, UserEvent userEvent, CancellationToken ct = default);
}

public class RewardPoolProcessor
{
    
}