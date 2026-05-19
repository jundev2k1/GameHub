using game_x.application.Features.Rewards.Dtos;
using game_x.domain.Entities.Rewards;

namespace game_x.application.Contract.Persistence.Repo.Reward;

public interface IRewardDefinitionRepo
{
    Task<RewardDefinitionDto[]> GetListAsync(CancellationToken ct = default);
    
    Task<bool> CheckExistedCodeAsync(string code, CancellationToken ct = default);
    
    Task<RewardDefinition> GetByIdAsync(Guid id, CancellationToken ct = default);
    
    Task AddAsync(RewardDefinition entity, CancellationToken ct = default);
    
    Task UpdateAsync(Guid id, Action<RewardDefinition> updateAction, CancellationToken ct = default);

    Task RemoveAsync(Guid id, CancellationToken ct = default);
}