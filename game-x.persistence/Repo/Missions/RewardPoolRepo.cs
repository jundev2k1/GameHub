using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo.Reward;

namespace game_x.persistence.Repo.Missions;

public sealed class RewardPoolRepo(GameXContext dbContext) : IRewardPoolRepo, IRepository
{

}