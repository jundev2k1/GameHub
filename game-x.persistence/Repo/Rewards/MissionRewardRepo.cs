using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;

namespace game_x.persistence.Repo.Rewards;

public sealed class MissionRewardRepo(GameXContext dbContext) : IMissionRewardRepo, IRepository
{
    public async Task AddAsync(MissionReward entity, CancellationToken ct = default)
    {
        await dbContext.MissionRewards.AddAsync(entity, ct);
    }
}