using game_x.application.Features.Rewards.Dtos;

namespace game_x.application.Contract.Infrastructure.Caching.Rewards;

public interface IMissionCacheService
{
    Task RefreshCache(CancellationToken ct = default);

    Task<MissionDto[]?> GetAll(CancellationToken ct = default);

    Task<MissionDto?> GetDetail(Guid id, CancellationToken ct = default);
}