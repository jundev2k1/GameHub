using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Features.Rewards.Dtos;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching.Rewards;

public sealed class MissionCacheService(
    IMemoryCache cache,
    IMissionRepo repo) : CacheService(cache), IMissionCacheService
{
    private MissionDto[]? Datasource => Get<MissionDto[]>($"{RewardCacheKey.Mission}:list");

    public async Task RefreshCache(CancellationToken ct = default)
    {
        var data = await repo.GetListAsync(ct);
        Set($"{RewardCacheKey.Mission}:list", data);
    }

    public async Task<MissionDto[]?> GetAll(CancellationToken ct = default)
    {
        if (Datasource == null) await RefreshCache(ct);
        return Datasource;
    }

    public async Task<MissionDto?> GetDetail(Guid id, CancellationToken ct = default)
    {
        if (Datasource == null) await RefreshCache(ct);
        return Datasource?.FirstOrDefault(x => x.Id == id);
    }
}