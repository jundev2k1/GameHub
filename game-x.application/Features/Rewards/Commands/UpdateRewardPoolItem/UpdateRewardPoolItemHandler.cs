using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.UpdateRewardPoolItem;

public sealed class UpdateRewardPoolItemHandler(
    IUnitOfWork unitOfWork,
    IRewardDefinitionRepo rewardRepo,
    IRewardPoolItemRepo itemRepo,
    IRewardPoolItemCacheService cache,
    ILogger<UpdateRewardPoolItemHandler> logger
    ) : ICommandHandler<UpdateRewardPoolItemCommand, Unit>
{
    public async Task<Unit> Handle(UpdateRewardPoolItemCommand cmd, CancellationToken ct = default)
    {
        var (item, reward) = await Validate(cmd, ct);
        
        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                await itemRepo.UpdateAsync(cmd.Id, x =>
                {
                    x.OnUpdate(
                        rewardDefinitionId: reward?.Id, 
                        weight: cmd.Weight, 
                        isActive: cmd.IsActive,
                        sortOrder: cmd.SortOrder,
                        startAt: cmd.StartAt,
                        endAt: cmd.EndAt);
                }, ct);
                await unitOfWork.CommitAsync(ct);
                await cache.RefreshCache(item.RewardPool!.PublicId, ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to update reward pool item.");
                throw new BadRequestException("Failed to update reward pool item.");
            }
        }, ct);
        
        return Unit.Value;
    }

    private async Task<(RewardPoolItem poolItem, RewardDefinition? reward)> Validate(UpdateRewardPoolItemCommand cmd, CancellationToken ct = default)
    {
        var item = await itemRepo.GetDetailByIdAsync(cmd.Id, ct);
        if (item.RewardPool == null)
            throw new NotFoundException(MessageCode.Reward.RewardPoolNotFound);
        
        var reward = cmd.RewardDefinitionId.HasValue
            ? await rewardRepo.GetDetailByIdAsync(cmd.RewardDefinitionId.Value, ct) 
            : null;
        
        return (item, reward);
    }
}