using game_x.application.Features.Rewards.Dtos;
using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;

namespace game_x.application.Contract.Persistence.Repo.Reward;

public interface IMissionRepo
{
    Task<ListedMissionDto[]> GetListAsync(CancellationToken ct = default);
    
    Task<MissionDto> GetDetailAsync(Guid id, CancellationToken ct = default);

    Task<UserMissionDto> GetDetailByUserAsync(string userId, Guid missionId, CancellationToken ct = default);
    
    Task<Mission> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
    
    Task<IReadOnlyCollection<Mission>> GetTriggeredByEventAsync(UserEventType eventType, CancellationToken ct = default);
    
    Task AddAsync(Mission entity, CancellationToken ct = default);

    Task UpdateAsync(Guid id, Action<Mission> updateAction, CancellationToken ct = default);
    
    Task RemoveAsync(Guid id, CancellationToken ct = default);
}