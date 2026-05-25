using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.CatalogItems.Remove;

public sealed class CatalogItemRemoveHandler(
    IUnitOfWork unitOfWork,
    ICatalogItemRepo catalogItemRepo,
    IRewardDefinitionRepo rewardRepo,
    ICatalogItemCacheService catalogCache,
    IRewardDefinitionCacheService rewardCache,
    ILogger<CatalogItemRemoveHandler> logger
    ) : ICommandHandler<CatalogItemRemoveCommand, Unit>
{
    public async Task<Unit> Handle(CatalogItemRemoveCommand cmd, CancellationToken ct = default)
    {
        var catalogItem = await catalogItemRepo.GetByIdAsync(cmd.Id, ct);
        if (catalogItem is null)
            throw new NotFoundException(MessageCode.Reward.CatalogNotFound);
        
        var isUsed = await rewardRepo.ExistsByCatalogItemIdAsync(catalogItem.Id, ct);

        if (isUsed)
            throw new BadRequestException(
                MessageCode.System.EntityInUse,
                new
                {
                    Message = $"Cannot delete catalog item '{catalogItem.Name}' because it is currently being used by reward definitions."
                }
            );
        
        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                await catalogItemRepo.RemoveAsync(cmd.Id, ct);
                await unitOfWork.CommitAsync(ct);
                await catalogCache.RefreshCache(ct);
                await rewardCache.RefreshCache(ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to remove catalog item.");
                throw new BadRequestException("Failed to remove catalog item.", e);
            }
        }, ct);
        
        return Unit.Value;
    }
}