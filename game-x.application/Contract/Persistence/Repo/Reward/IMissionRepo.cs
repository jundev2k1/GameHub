using game_x.application.Features.Rewards.Dtos;
using game_x.domain.Entities.Rewards;

namespace game_x.application.Contract.Persistence.Repo.Reward;

public interface IMissionRepo
{
    Task<MissionDto[]> GetListAsync(CancellationToken ct = default);
    
    Task<MissionDto> GetDetailAsync(Guid id, CancellationToken ct = default);
    
    Task AddAsync(Mission entity, CancellationToken ct = default);
}