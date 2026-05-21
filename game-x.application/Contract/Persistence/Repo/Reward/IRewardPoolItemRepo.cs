using System.Linq.Expressions;
using game_x.application.Features.Rewards.Dtos;
using game_x.domain.Entities.Rewards;

namespace game_x.application.Contract.Persistence.Repo.Reward;

public interface IRewardPoolItemRepo
{
    Task<RewardPoolItemDto[]> GetListAsync(int poolId, CancellationToken ct = default);
    
    Task<RewardPoolItem> GetDetailByIdAsync(Guid id, CancellationToken ct = default);
    
    Task<IReadOnlyCollection<RewardPoolItem>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    
    Task<ICollection<RewardPoolItem>> GetByIdsForUpdateAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    
    Task AddRangeAsync(IEnumerable<RewardPoolItem> items, CancellationToken ct = default);
    
    Task BulkDeleteAsync(Expression<Func<RewardPoolItem, bool>> predicate, CancellationToken ct = default);
}