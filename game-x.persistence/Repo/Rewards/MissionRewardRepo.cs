using System.Linq.Expressions;
using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;

namespace game_x.persistence.Repo.Rewards;

public sealed class MissionRewardRepo(GameXContext dbContext) : IMissionRewardRepo, IRepository
{
    public async Task<IReadOnlyCollection<MissionReward>> GetByMissionIdAsync(int missionId, CancellationToken ct = default)
    {
        return await dbContext.MissionRewards
            .AsNoTracking()
            .Where(missionReward => missionReward.MissionId == missionId)
            .ToListAsync(ct);
    }
    
    public async Task<ICollection<MissionReward>> GetByIdsForUpdateAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        return await dbContext.MissionRewards
            .Where(x => ids.Contains(x.PublicId))
            .ToListAsync(ct);
    }
    
    public Task<bool> ExistsByRewardIdAsync(int rewardId, CancellationToken ct = default)
    {
        return dbContext.MissionRewards
            .AnyAsync(x => x.RewardDefinitionId == rewardId, ct);
    }
    
    public async Task AddRangeAsync(IEnumerable<MissionReward> items, CancellationToken ct = default)
    {
        await dbContext.MissionRewards.AddRangeAsync(items, ct);
    }
    
    public async Task BulkDeleteAsync(Expression<Func<MissionReward, bool>> predicate, CancellationToken ct = default)
    {
        await dbContext.MissionRewards.Where(predicate).ExecuteDeleteAsync(ct);
    }
}