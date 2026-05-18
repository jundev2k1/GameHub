using game_x.application.Features.Rewards.Dtos;
using game_x.domain.Entities.Rewards;

namespace game_x.application.Contract.Persistence.Repo.Reward;

public interface IRewardPoolItemRepo
{
    Task<RewardPoolItemDto[]> GetListAsync(int poolId, CancellationToken ct = default);
    
    Task<RewardPoolItem> GetDetailByIdAsync(Guid id, CancellationToken ct = default);
    
    Task AddAsync(RewardPoolItem entity, CancellationToken ct = default);

    Task UpdateAsync(Guid id, Action<RewardPoolItem> updateAction, CancellationToken ct = default);
    
    Task RemoveAsync(Guid id, CancellationToken ct = default);
}