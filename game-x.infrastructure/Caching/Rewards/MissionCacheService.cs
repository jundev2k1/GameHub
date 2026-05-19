using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Features.Rewards.Dtos;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching.Rewards;

public sealed class MissionCacheService(
    IMemoryCache cache,
    IMissionRepo repo) : CacheService(cache), IMissionCacheService
{
    private ListedMissionDto[]? Datasource => Get<ListedMissionDto[]>($"{RewardCacheKey.Mission}:list");

    public async Task RefreshCache(CancellationToken ct = default)
    {
        var data = await repo.GetListAsync(ct);
        Set($"{RewardCacheKey.Mission}:list", data);
    }
    
    public async Task RefreshCache(Guid id, CancellationToken ct = default)
    {
        var data = await repo.GetDetailAsync(id, ct);
        Set($"{RewardCacheKey.Mission}:{id}:detail", data);
    }

    public async Task<ListedMissionDto[]?> GetAll(CancellationToken ct = default)
    {
        if (Datasource == null) await RefreshCache(ct);
        return Datasource;
    }

    public async Task<MissionDto?> GetDetail(Guid id, CancellationToken ct = default)
    {
        string key = $"{RewardCacheKey.Mission}:{id}:detail";
        var mission = Get<MissionDto>(key);
        if (mission == null) await RefreshCache(id, ct);
        return Get<MissionDto>(key);
    }
}