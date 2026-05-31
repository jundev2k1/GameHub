using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Features.Rewards.Dtos;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching.Rewards;

public sealed class RewardDefinitionCacheService(
    IMemoryCache cache,
    IRewardDefinitionRepo repo) : CacheService(cache), IRewardDefinitionCacheService
{
    private RewardDefinitionDto[]? Datasource => Get<RewardDefinitionDto[]>($"{RewardCacheKey.RewardDefinition}:list");

    public async Task RefreshCache(CancellationToken ct = default)
    {
        var data = await repo.GetListAsync(ct);
        Set($"{RewardCacheKey.RewardDefinition}:list", data);
    }

    public async Task<RewardDefinitionDto[]?> GetAll(CancellationToken ct = default)
    {
        if (Datasource == null) await RefreshCache(ct);
        return Datasource;
    }
}