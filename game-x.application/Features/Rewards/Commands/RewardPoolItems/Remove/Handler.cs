using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.RewardPoolItems.Remove;

public sealed class RemoveRewardPoolItemHandler(
    IUnitOfWork unitOfWork,
    IRewardPoolItemRepo itemRepo,
    IRewardPoolItemCacheService cache,
    ILogger<RemoveRewardPoolItemHandler> logger
    ) : ICommandHandler<RemoveRewardPoolItemCommand, Unit>
{
    public async Task<Unit> Handle(RemoveRewardPoolItemCommand cmd, CancellationToken ct = default)
    {
        var item = await itemRepo.GetDetailByIdAsync(cmd.Id, ct);
        if (item.RewardPool == null)
            throw new NotFoundException(MessageCode.Reward.RewardPoolNotFound);
        
        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                await itemRepo.RemoveAsync(item.PublicId, ct);
                await unitOfWork.CommitAsync(ct);
                await cache.RefreshCache(item.RewardPool.PublicId, ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to remove pool item.");
                throw new BadRequestException("Failed to remove pool item.", e);
            }
        }, ct);
        
        return Unit.Value;
    }
}