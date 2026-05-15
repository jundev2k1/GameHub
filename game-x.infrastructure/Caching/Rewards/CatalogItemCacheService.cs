using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Features.Rewards.Dtos;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching.Rewards;

public sealed class CatalogItemCacheService(
    IMemoryCache cache,
    ICatalogItemRepo repo) : CacheService(cache), ICatalogItemCacheService
{
    private CatalogItemDto[]? Datasource => Get<CatalogItemDto[]>(RewardCacheKey.CatalogItem);

    public async Task RefreshCache(CancellationToken ct = default)
    {
        var data = await repo.GetListAsync(ct);
        Set(RewardCacheKey.CatalogItem, data);
    }

    public async Task<CatalogItemDto[]?> GetAll(CancellationToken ct = default)
    {
        if (Datasource == null) await RefreshCache(ct);
        return Datasource;
    }
}