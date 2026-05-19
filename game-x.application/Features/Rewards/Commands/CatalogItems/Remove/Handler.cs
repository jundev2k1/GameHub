using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.CatalogItems.Create.Remove;

public sealed class CatalogItemRemoveHandler(
    IUnitOfWork unitOfWork,
    ICatalogItemRepo repo,
    ICatalogItemCacheService catalogCache,
    IRewardDefinitionCacheService rewardCache,
    ILogger<CatalogItemRemoveHandler> logger
    ) : ICommandHandler<CatalogItemRemoveCommand, Unit>
{
    public async Task<Unit> Handle(CatalogItemRemoveCommand cmd, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                await repo.UpdateAsync(cmd.Id, x => { x.SoftDelete(); }, ct);
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