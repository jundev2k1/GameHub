using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.RewardPools.SyncItems;

public sealed class SyncRewardPoolItemHandler(
    IUnitOfWork unitOfWork,
    IRewardPoolRepo poolRepo,
    IRewardDefinitionRepo rewardRepo,
    IRewardPoolItemRepo itemRepo,
    IRewardPoolItemCacheService cache,
    ILogger<SyncRewardPoolItemHandler> logger
) : ICommandHandler<SyncRewardPoolItemCommand, Unit>
{
    public async Task<Unit> Handle(SyncRewardPoolItemCommand cmd, CancellationToken ct = default)
    {
        var context = await Validate(cmd, ct);

        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                await DeleteItems(cmd, ct);
                UpdateItems(cmd, context);
                await CreateItems(cmd, context, ct);
                await unitOfWork.SaveChangesAsync(ct);
                await cache.RefreshCache(context.RewardPool.PublicId, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to sync reward pool items.");
                throw;
            }
        }, ct);

        return Unit.Value;
    }

    private async Task DeleteItems(SyncRewardPoolItemCommand cmd, CancellationToken ct)
    {
        if (cmd.DeletedItems.Count == 0) return;
        await itemRepo.BulkDeleteAsync(x => cmd.DeletedItems.Contains(x.PublicId), ct);
    }

    private void UpdateItems(SyncRewardPoolItemCommand cmd, ValidationContext context)
    {
        foreach (var dto in cmd.UpdatedItems)
        {
            var item = context.ItemLookup[dto.Id]; RewardDefinition? reward = null;
            if (dto.RewardDefinitionId.HasValue) reward = context.RewardLookup[dto.RewardDefinitionId.Value];
            item.OnUpdate(
                rewardDefinitionId: reward?.Id,
                weight: dto.Weight,
                isActive: dto.IsActive,
                sortOrder: dto.SortOrder,
                startAt: dto.StartAt,
                endAt: dto.EndAt);
        }
    }

    private async Task CreateItems(SyncRewardPoolItemCommand cmd, ValidationContext context, CancellationToken ct)
    {
        if (cmd.CreatedItems.Count == 0) return;
        var newItems = cmd.CreatedItems
            .Select(dto =>
            {
                var reward = context.RewardLookup[dto.RewardDefinitionId];
                return RewardPoolItem.Create(
                    rewardPoolId: context.RewardPool.Id,
                    rewardDefinitionId: reward.Id,
                    weight: dto.Weight,
                    sortOrder: dto.SortOrder,
                    startAt: dto.StartAt,
                    endAt: dto.EndAt);
            })
            .ToList();
        await itemRepo.AddRangeAsync(newItems, ct);
    }

    private async Task<ValidationContext> Validate(SyncRewardPoolItemCommand cmd, CancellationToken ct)
    {
        var rewardPool = await poolRepo.GetDetailByIdAsync(cmd.RewardPoolId, ct);
        var rewardDefinitionIds = cmd.CreatedItems
            .Select(x => x.RewardDefinitionId)
            .Concat(
                cmd.UpdatedItems
                    .Where(x => x.RewardDefinitionId.HasValue)
                    .Select(x => x.RewardDefinitionId!.Value))
            .Distinct()
            .ToHashSet();

        var rewards = rewardDefinitionIds.Count == 0
            ? []
            : await rewardRepo.GetByIdsAsync(rewardDefinitionIds, ct);

        var rewardLookup = rewards.ToDictionary(x => x.PublicId);
        var missingRewardIds = rewardDefinitionIds
            .Where(id => !rewardLookup.ContainsKey(id))
            .ToList();

        if (missingRewardIds.Count > 0)
            throw new NotFoundException(MessageCode.Reward.RewardDefinitionNotFound, new { MissingIds = missingRewardIds });

        var inactiveRewardIds = rewards
            .Where(x => !x.IsActive)
            .Select(x => x.PublicId)
            .ToList();

        if (inactiveRewardIds.Count > 0)
            throw new BadRequestException(
                MessageCode.Reward.RewardDefinitionInactive,
                new { InvalidIds = inactiveRewardIds });
        
        var itemIds = cmd.UpdatedItems
            .Select(x => x.Id)
            .Concat(cmd.DeletedItems)
            .Distinct()
            .ToHashSet();

        var items = itemIds.Count == 0
            ? []
            : await itemRepo.GetByIdsForUpdateAsync(itemIds, ct);

        var itemLookup = items.ToDictionary(x => x.PublicId);

        var missingItemIds = itemIds
            .Where(id => !itemLookup.ContainsKey(id))
            .ToList();

        if (missingItemIds.Count > 0)
            throw new NotFoundException(
                MessageCode.Reward.RewardPoolItemNotFound,
                new { MissingIds = missingItemIds });

        var invalidPoolItemIds = items
            .Where(x => x.RewardPool?.PublicId != cmd.RewardPoolId)
            .Select(x => x.PublicId)
            .ToList();

        if (invalidPoolItemIds.Count > 0)
            throw new BadRequestException(MessageCode.Reward.RewardPoolItemInvalid, new { InvalidIds = invalidPoolItemIds });

        return new ValidationContext(rewardPool, rewardLookup, itemLookup);
    }

    private sealed record ValidationContext(
        RewardPool RewardPool,
        Dictionary<Guid, RewardDefinition> RewardLookup,
        Dictionary<Guid, RewardPoolItem> ItemLookup);
}