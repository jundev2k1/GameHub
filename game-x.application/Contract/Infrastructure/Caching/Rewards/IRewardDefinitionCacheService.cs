using game_x.application.Features.Rewards.Dtos;

namespace game_x.application.Contract.Infrastructure.Caching.Rewards;

public interface IRewardDefinitionCacheService
{
    Task RefreshCache(CancellationToken ct = default);

    Task<RewardDefinitionDto[]?> GetAll(CancellationToken ct = default);
}