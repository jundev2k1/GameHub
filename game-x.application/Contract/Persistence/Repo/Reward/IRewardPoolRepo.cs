using game_x.application.Features.Rewards.Dtos;
using game_x.domain.Entities.Rewards;

namespace game_x.application.Contract.Persistence.Repo.Reward;

public interface IRewardPoolRepo
{
    Task<RewardPoolDto[]> GetListAsync(CancellationToken ct = default);
    
    Task<RewardPool> GetDetailByIdAsync(Guid id, CancellationToken ct = default);
    
    Task<bool> CheckExistedCodeAsync(string code, CancellationToken ct = default);
    
    Task UpdateAsync(Guid id, Action<RewardPool> updateAction, CancellationToken ct = default);
}