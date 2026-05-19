using game_x.application.Features.Rewards.Dtos;

namespace game_x.application.Contract.Infrastructure.Caching.Rewards;

public interface IMissionCacheService
{
    Task RefreshCache(CancellationToken ct = default);
    
    Task RefreshCache(Guid id, CancellationToken ct = default);

    Task<ListedMissionDto[]?> GetAll(CancellationToken ct = default);

    Task<MissionDto?> GetDetail(Guid id, CancellationToken ct = default);
}