using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Features.Rewards.Dtos;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching.Rewards;

public sealed class MissionCacheService(
    IMemoryCache cache,
    IMissionRepo repo,
    IFileManagerCacheService storage) : CacheService(cache), IMissionCacheService
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
        var missionRewards = await Task.WhenAll(
            data.MissionRewards.Select(async item =>
            {
                var dto = item.Adapt<MissionRewardDetailDto>();
                dto.ItemIconUrl = item.ItemIcon is null
                    ? null
                    : await storage.GetFileUrl(item.ItemIcon, ct);

                return dto;
            }));

        var missionDto = data.Adapt<MissionDetailDto>();
        missionDto.MissionRewards = missionRewards;
        Set($"{RewardCacheKey.Mission}:{id}:detail", missionDto);
    }
    
    public async Task<ListedMissionDto[]?> GetAll(CancellationToken ct = default)
    {
        if (Datasource == null) await RefreshCache(ct);
        return Datasource;
    }

    public async Task<MissionDetailDto?> GetDetail(Guid id, CancellationToken ct = default)
    {
        string key = $"{RewardCacheKey.Mission}:{id}:detail";
        var mission = Get<MissionDetailDto>(key);
        if (mission == null) await RefreshCache(id, ct);
        return Get<MissionDetailDto>(key);
    }

    public void RemoveGetDetail(Guid id)
    {
        Remove($"{RewardCacheKey.Mission}:{id}:detail");
    }
}