using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo.Reward;

namespace game_x.persistence.Repo.Missions;

public sealed class UserMissionRepo(GameXContext dbContext) : IUserMissionRepo, IRepository
{

}