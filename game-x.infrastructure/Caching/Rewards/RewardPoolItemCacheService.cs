using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Features.Rewards.Dtos;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching.Rewards;

public sealed class RewardPoolItemCacheService(
    IMemoryCache cache,
    IRewardPoolRepo poolRepo,
    IRewardPoolItemRepo itemRepo) : CacheService(cache), IRewardPoolItemCacheService
{
    private string GetKey(Guid poolId) => $"{RewardCacheKey.RewardPool}/{poolId}/{RewardCacheKey.RewardPoolItem}";
    private RewardPoolItemDto[]? Datasource(Guid poolId) => Get<RewardPoolItemDto[]>($"{GetKey(poolId)}:list");
    
    public async Task RefreshCache(Guid poolId, CancellationToken ct = default)
    {
        var pool = await poolRepo.GetDetailByIdAsync(poolId, ct);
        var data = await itemRepo.GetListAsync(pool.Id, ct);
        Set($"{GetKey(poolId)}:list", data);
    }

    public async Task<RewardPoolItemDto[]?> GetAll(Guid poolId, CancellationToken ct = default)
    {
        if (Datasource(poolId) == null) await RefreshCache(poolId, ct);
        return Datasource(poolId);
    }
}