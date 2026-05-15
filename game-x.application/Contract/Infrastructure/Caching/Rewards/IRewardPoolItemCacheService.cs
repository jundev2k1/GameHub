using game_x.application.Features.Rewards.Dtos;

namespace game_x.application.Contract.Infrastructure.Caching.Rewards;

public interface IRewardPoolItemCacheService
{
    Task RefreshCache(Guid poolId, CancellationToken ct = default);

    Task<RewardPoolItemDto[]?> GetAll(Guid poolId, CancellationToken ct = default);
}