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
    private string ListByAdminKey(Guid poolId) => $"{RewardCacheKey.RewardPool}/{poolId}/{RewardCacheKey.RewardPoolItem}:admin-list";
    private string ListByUserKey(Guid poolId) => $"{RewardCacheKey.RewardPool}/{poolId}/{RewardCacheKey.RewardPoolItem}:user-list";
    
    public async Task RefreshCache(Guid poolId, CancellationToken ct = default)
    {
        var pool = await poolRepo.GetDetailByIdAsync(poolId, ct);
        var dataAdmin = await itemRepo.GetAllByAdminAsync(pool.Id, ct);
        var dataUser = await itemRepo.GetAllByUserAsync(pool.Id, ct);
        var dataUserDto = dataUser
            .Select(x => x.Adapt<RewardPoolItemUserDto>())
            .ToArray();
        Set(ListByAdminKey(poolId), dataAdmin);
        Set(ListByUserKey(poolId), dataUserDto);
    }

    public async Task<RewardPoolItemDto[]?> GetAllByAdmin(Guid poolId, CancellationToken ct = default)
    {
        var data = Get<RewardPoolItemDto[]>(ListByAdminKey(poolId));
        if (data == null) await RefreshCache(poolId, ct);
        return Get<RewardPoolItemDto[]>(ListByAdminKey(poolId));
    }

    public async Task<RewardPoolItemUserDto[]?> GetAllByUser(Guid poolId, CancellationToken ct = default)
    {
        var data = Get<RewardPoolItemUserDto[]>(ListByUserKey(poolId));
        if (data == null) await RefreshCache(poolId, ct);
        var newTest = Get<RewardPoolItemUserDto[]>(ListByUserKey(poolId));
        return newTest;
    }
}