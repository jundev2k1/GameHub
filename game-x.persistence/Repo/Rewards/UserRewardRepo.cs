using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;

namespace game_x.persistence.Repo.Rewards;

public sealed class UserRewardRepo(GameXContext dbContext) : IUserRewardRepo, IRepository
{
    public async Task AddAsync(UserReward entity, CancellationToken ct = default)
    {
        await dbContext.UserRewards.AddAsync(entity, ct);
    }
}