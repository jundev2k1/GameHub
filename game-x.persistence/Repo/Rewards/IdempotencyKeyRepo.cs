using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo.Reward;

namespace game_x.persistence.Repo.Rewards;

public sealed class IdempotencyKeyRepo(GameXContext dbContext) : IIdempotencyKeyRepo, IRepository
{

}