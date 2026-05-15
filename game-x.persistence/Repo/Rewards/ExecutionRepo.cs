using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;

namespace game_x.persistence.Repo.Rewards;

public sealed class ExecutionRepo(GameXContext dbContext) : IExecutionRepo, IRepository
{
    public async Task AddAsync(Execution entity, CancellationToken ct = default)
    {
        await dbContext.Executions.AddAsync(entity, ct);
    }
}