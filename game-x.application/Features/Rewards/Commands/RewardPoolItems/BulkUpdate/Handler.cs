using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.RewardPoolItems.BulkUpdate;

public sealed class BulkUpdateRewardPoolItemHandler(
    IUnitOfWork unitOfWork,
    IRewardDefinitionRepo rewardRepo,
    IRewardPoolItemRepo itemRepo,
    IRewardPoolItemCacheService cache,
    ILogger<BulkUpdateRewardPoolItemHandler> logger
) : ICommandHandler<BulkUpdateRewardPoolItemCommand, Unit>
{
    public async Task<Unit> Handle(
        BulkUpdateRewardPoolItemCommand cmd,
        CancellationToken ct = default)
    {
        var (items, rewards) = await Validate(cmd, ct);

        var itemLookup = items.ToDictionary(x => x.PublicId);
        var rewardLookup = rewards.ToDictionary(x => x.PublicId);

        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                foreach (var dto in cmd.Items)
                {
                    rewardLookup.TryGetValue(dto.RewardDefinitionId ?? Guid.Empty, out var reward);

                    itemLookup[dto.Id].OnUpdate(
                        rewardDefinitionId: reward?.Id,
                        weight: dto.Weight,
                        isActive: dto.IsActive,
                        sortOrder: dto.SortOrder,
                        startAt: dto.StartAt,
                        endAt: dto.EndAt);
                }

                await unitOfWork.CommitAsync(ct);
                await cache.RefreshCache(cmd.RewardPoolId, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to bulk update reward pool items.");
                throw;
            }
        }, ct);

        return Unit.Value;
    }

    private async Task<
        (ICollection<RewardPoolItem> items,
         IReadOnlyCollection<RewardDefinition> rewards)>
        Validate(
            BulkUpdateRewardPoolItemCommand cmd,
            CancellationToken ct)
    {
        var itemIds = cmd.Items.Select(x => x.Id).ToList();

        var items = await itemRepo.GetByIdsForUpdateAsync(itemIds, ct);

        if (items.Count != itemIds.Count)
            throw new NotFoundException(MessageCode.Reward.RewardPoolItemNotFound);

        foreach(var item in items)
        {
            if(item.RewardPool?.PublicId != cmd.RewardPoolId)
                throw new BadRequestException(
                    MessageCode.Reward.RewardPoolItemInvalid, 
                    new {Message = $"Reward pool item {item.PublicId} is invalid."});
        }

        var rewardIds = cmd.Items
            .Where(x => x.RewardDefinitionId.HasValue)
            .Select(x => x.RewardDefinitionId!.Value)
            .Distinct()
            .ToList();

        var rewards = rewardIds.Count == 0
            ? []
            : await rewardRepo.GetByIdsAsync(rewardIds, ct);

        foreach (var item in items)
        {
            if(!item.IsActive)
                throw new BadRequestException(
                    MessageCode.Reward.RewardDefinitionInactive,
                    new {Message = $"Reward definition {item.PublicId} is invalid."});
        }
        
        return (items, rewards);
    }
}