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
    private string ListByAdminKey => $"{RewardCacheKey.Mission}:admin-list";
    private string ListByUserKey => $"{RewardCacheKey.Mission}:user-list";
    private string DetailByAdminKey(Guid id) => $"{RewardCacheKey.Mission}:{id}:admin-detail";
    
    public async Task RefreshCache(CancellationToken ct = default)
    {
        var dataByAdmin = await repo.GetAllForAdminAsync(ct);
        var dataByUser = await repo.GetAllForUserAsync(ct);
        Set(ListByAdminKey, dataByAdmin);
        Set(ListByUserKey, dataByUser);
    }
    
    public async Task RefreshCache(Guid id, CancellationToken ct = default)
    {
        var data = await repo.GetDetailForAdminAsync(id, ct);
        var missionRewards = await Task.WhenAll(
            data.MissionRewards.Select(async item =>
            {
                var dto = item.Adapt<MissionRewardAdminDto>();
                dto.ItemIconUrl = item.ItemIcon is null
                    ? null
                    : await storage.GetFileUrl(item.ItemIcon, ct);

                return dto;
            }));

        var missionDto = data.Adapt<MissionAdminDto>();
        missionDto.MissionRewards = missionRewards;
        Set(DetailByAdminKey(id), missionDto);
    }
    
    public async Task<MissionListedAdminDto[]?> GetAllByAdmin(CancellationToken ct = default)
    {
        var data = Get<MissionListedAdminDto[]>(ListByAdminKey);
        if (data == null) await RefreshCache(ct);
        return Get<MissionListedAdminDto[]>(ListByAdminKey);
    }
    
    public async Task<MissionListedUserDto[]?> GetAllByUser(CancellationToken ct = default)
    {
        var data = Get<MissionListedUserDto[]>(ListByUserKey);
        if (data == null) await RefreshCache(ct);
        return Get<MissionListedUserDto[]>(ListByUserKey);
    }

    public async Task<MissionAdminDto?> GetDetailByAdmin(Guid id, CancellationToken ct = default)
    {
        string key = DetailByAdminKey(id);
        var mission = Get<MissionAdminDto>(key);
        if (mission == null) await RefreshCache(id, ct);
        return Get<MissionAdminDto>(key);
    }

    public void RemoveGetDetailByAdmin(Guid id)
    {
        Remove(DetailByAdminKey(id));
    }
}