using game_x.application.Features.Rewards.Dtos;

namespace game_x.application.Contract.Infrastructure.Caching.Rewards;

public interface IMissionCacheService
{
    Task RefreshCache(CancellationToken ct = default);
    
    Task RefreshCache(Guid id, CancellationToken ct = default);

    Task<MissionListedAdminDto[]?> GetAllByAdmin(CancellationToken ct = default);
    
    Task<MissionListedUserDto[]?> GetAllByUser(CancellationToken ct = default);

    Task<MissionAdminDto?> GetDetailByAdmin(Guid id, CancellationToken ct = default);
    
    void RemoveGetDetailByAdmin(Guid id);
}