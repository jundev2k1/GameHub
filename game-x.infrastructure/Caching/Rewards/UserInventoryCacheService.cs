using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Features.Rewards.Dtos;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching.Rewards;

public sealed class UserInventoryCacheService(
    IMemoryCache cache,
    IUserInventoryRepo repo) : CacheService(cache), IUserInventoryCacheService
{
    private string GetKey(string userId) => $"users/{userId}/{RewardCacheKey.UserInventory}";
    private UserInventoryDto[]? Datasource(string userId) => Get<UserInventoryDto[]>($"{GetKey(userId)}:list");
    
    public async Task RefreshCache(string userId, CancellationToken ct = default)
    {
        var data = await repo.GetListAsync(userId, ct);
        Set($"{GetKey(userId)}:list", data);
    }

    public async Task<UserInventoryDto[]?> GetAll(string userId, CancellationToken ct = default)
    {
        if (Datasource(userId) == null) await RefreshCache(userId, ct);
        return Datasource(userId);
    }
}