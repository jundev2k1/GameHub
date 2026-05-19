using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.RewardDefinitions.Remove;

public sealed class RemoveRewardDefinitionHandler(
    IUnitOfWork unitOfWork,
    IRewardDefinitionRepo repo,
    IRewardDefinitionCacheService cache,
    ILogger<RemoveRewardDefinitionHandler> logger
    ) : ICommandHandler<RemoveRewardDefinitionCommand, Unit>
{
    public async Task<Unit> Handle(RemoveRewardDefinitionCommand cmd, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                await repo.UpdateAsync(cmd.Id, x => { x.SoftDelete(); }, ct);
                await unitOfWork.CommitAsync(ct);
                await cache.RefreshCache(ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to remove reward definition.");
                throw new BadRequestException("Failed to remove reward definition.", e);
            }
        }, ct);
        
        return Unit.Value;
    }
}