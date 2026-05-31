using game_x.application.Features.Rewards.Dtos;

namespace game_x.application.Contract.Infrastructure.Caching.Rewards;

public interface IRewardPoolCacheService
{
    Task RefreshCache(CancellationToken ct = default);

    Task<RewardPoolDto[]?> GetAll(CancellationToken ct = default);
    
    Task<RewardPoolDto?> GetDetail(Guid id, CancellationToken ct = default);
}