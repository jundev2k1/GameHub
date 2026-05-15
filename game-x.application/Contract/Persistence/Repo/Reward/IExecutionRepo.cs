using game_x.domain.Entities.Rewards;

namespace game_x.application.Contract.Persistence.Repo.Reward;

public interface IExecutionRepo
{
    Task AddAsync(Execution entity, CancellationToken ct = default);
}