using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.RewardDefinitions.Remove;

public sealed class RemoveRewardDefinitionHandler(
    IUnitOfWork unitOfWork,
    IRewardDefinitionRepo rewardRepo,
    IRewardPoolItemRepo rewardPoolItemRepo,
    IMissionRewardRepo missionRewardRepo,
    IRewardDefinitionCacheService cache,
    ILogger<RemoveRewardDefinitionHandler> logger
    ) : ICommandHandler<RemoveRewardDefinitionCommand, Unit>
{
    public async Task<Unit> Handle(RemoveRewardDefinitionCommand cmd, CancellationToken ct = default)
    {
        var reward = await rewardRepo.GetByIdAsync(cmd.Id, ct);
        if (reward is null)
            throw new NotFoundException(MessageCode.Reward.RewardDefinitionNotFound);
        
        var isUsedByPool = await rewardPoolItemRepo.ExistsByRewardIdAsync(reward.Id, ct);
        var isUsedByMission = await missionRewardRepo.ExistsByRewardIdAsync(reward.Id, ct);

        if (isUsedByPool || isUsedByMission)
            throw new BadRequestException(
                MessageCode.System.EntityInUse,
                new
                {
                    Message = $"Cannot delete reward '{reward.Title}' because it is currently being used."
                }
            );
        
        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                await rewardRepo.RemoveAsync(cmd.Id, ct);
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