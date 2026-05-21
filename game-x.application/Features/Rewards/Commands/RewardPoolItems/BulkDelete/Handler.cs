using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.RewardPoolItems.BulkDelete;

public sealed class BulkDeleteRewardPoolItemHandler(
    IUnitOfWork unitOfWork,
    IRewardPoolItemRepo itemRepo,
    IRewardPoolItemCacheService cache,
    ILogger<BulkDeleteRewardPoolItemHandler> logger
) : ICommandHandler<BulkDeleteRewardPoolItemCommand, Unit>
{
    public async Task<Unit> Handle(BulkDeleteRewardPoolItemCommand cmd, CancellationToken ct = default)
    {
         await Validate(cmd, ct);
        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                await itemRepo.BulkDeleteAsync(x => cmd.ItemIds.Contains(x.PublicId), ct);
                await unitOfWork.CommitAsync(ct);
                await cache.RefreshCache(cmd.RewardPoolId, ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to bulk delete reward pool items.");
                throw;
            }
        }, ct);

        return Unit.Value;
    }

    private async Task Validate(BulkDeleteRewardPoolItemCommand cmd, CancellationToken ct)
    {
        var items = await itemRepo.GetByIdsAsync(cmd.ItemIds, ct);

        if (items.Count != cmd.ItemIds.Count) 
            throw new NotFoundException(MessageCode.Reward.RewardPoolItemNotFound);

        foreach (var item in items)
        {
            if (item.RewardPool?.PublicId != cmd.RewardPoolId)
            {
                throw new BadRequestException(
                    MessageCode.Reward.RewardPoolItemInvalid,
                    new
                    {
                        Message = $"Reward pool item {item.PublicId} does not belong to this reward pool."
                    });
            }
        }
    }
}