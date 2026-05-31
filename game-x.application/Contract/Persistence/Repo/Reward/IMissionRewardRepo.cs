using System.Linq.Expressions;
using game_x.domain.Entities.Rewards;

namespace game_x.application.Contract.Persistence.Repo.Reward;

public interface IMissionRewardRepo
{
    Task<IReadOnlyCollection<MissionReward>> GetByMissionIdAsync(int missionId, CancellationToken ct = default);

    Task<ICollection<MissionReward>> GetByIdsForUpdateAsync(IEnumerable<Guid> ids, CancellationToken ct = default);

    public Task<bool> ExistsByRewardIdAsync(int rewardId, CancellationToken ct = default);
    
    Task AddRangeAsync(IEnumerable<MissionReward> items, CancellationToken ct = default);
    
    Task BulkDeleteAsync(Expression<Func<MissionReward, bool>> predicate, CancellationToken ct = default);
}