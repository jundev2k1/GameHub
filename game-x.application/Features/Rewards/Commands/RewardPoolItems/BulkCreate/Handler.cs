using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.RewardPoolItems.BulkCreate;

public sealed class BulkCreateRewardPoolItemHandler(
    IUnitOfWork unitOfWork,
    IRewardPoolItemRepo itemRepo,
    IRewardPoolRepo poolRepo,
    IRewardDefinitionRepo rewardRepo,
    IRewardPoolItemCacheService cache,
    ILogger<BulkCreateRewardPoolItemHandler> logger
) : ICommandHandler<BulkCreateRewardPoolItemCommand, Unit>
{
    public async Task<Unit> Handle(
        BulkCreateRewardPoolItemCommand cmd,
        CancellationToken ct = default)
    {
        var (rewardPool, rewardLookup) = await Validate(cmd, ct);

        var items = cmd.Items
            .Select(dto =>
            {
                var reward = rewardLookup[dto.RewardDefinitionId];
                return RewardPoolItem.Create(
                    rewardPoolId: rewardPool.Id,
                    rewardDefinitionId: reward.Id,
                    weight: dto.Weight,
                    sortOrder: dto.SortOrder,
                    startAt: dto.StartAt,
                    endAt: dto.EndAt
                );
            })
            .ToList();

        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                await itemRepo.AddRangeAsync(items, ct);
                await unitOfWork.SaveChangesAsync(ct);
                await cache.RefreshCache(rewardPool.PublicId, ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to create reward pool items.");
                throw;
            }
        }, ct);

        return Unit.Value;
    }

    private async Task<(RewardPool rewardPool, Dictionary<Guid, RewardDefinition> rewardLookup)> 
        Validate(BulkCreateRewardPoolItemCommand cmd, CancellationToken ct)
    {
        var rewardPool = await poolRepo.GetDetailByIdAsync(cmd.RewardPoolId, ct);
        var rewardIds = cmd.Items
            .Select(x => x.RewardDefinitionId)
            .Distinct()
            .ToHashSet();

        var rewards = await rewardRepo.GetByIdsAsync(rewardIds, ct);
        var rewardLookup = rewards.ToDictionary(x => x.PublicId);

        var missingRewardIds = rewardIds
            .Where(id => !rewardLookup.ContainsKey(id))
            .ToList();

        if (missingRewardIds.Count > 0)
        {
            throw new NotFoundException(
                MessageCode.Reward.RewardDefinitionNotFound,
                new
                {
                    Message = $"Reward definitions not found: {string.Join(", ", missingRewardIds)}"
                });
        }

        var inactiveReward = rewards.FirstOrDefault(x => !x.IsActive);
        if (inactiveReward is not null)
        {
            throw new BadRequestException(
                MessageCode.Reward.RewardDefinitionInactive,
                new
                {
                    Message = $"Reward definition {inactiveReward.PublicId} is inactive."
                });
        }

        return (rewardPool, rewardLookup);
    }
}