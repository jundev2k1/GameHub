using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.RewardDefinitions.Update;

public sealed class UpdateRewardDefinitionHandler(
    IUnitOfWork unitOfWork,
    ICatalogItemRepo catalogRepo,
    IRewardDefinitionRepo rewardRepo,
    IRewardDefinitionCacheService cache,
    ILogger<UpdateRewardDefinitionHandler> logger
    ) : ICommandHandler<UpdateRewardDefinitionCommand, Unit>
{
    public async Task<Unit> Handle(UpdateRewardDefinitionCommand cmd, CancellationToken ct = default)
    {
        var catalog = await Validate(cmd, ct);
        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                await rewardRepo.UpdateAsync(cmd.Id, x =>
                {
                    x.OnUpdate(
                        code: cmd.Code,
                        catalogItemId: catalog?.Id,
                        title: cmd.Title, 
                        description: cmd.Description,
                        type: cmd.Type,
                        isActive: cmd.IsActive,
                        amount: cmd.Amount,
                        metadata: cmd.Metadata);
                }, ct);
                await unitOfWork.CommitAsync(ct);
                await cache.RefreshCache(ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to update reward definition.");
                throw;
            }
        }, ct);
        
        return Unit.Value;
    }

    private async Task<CatalogItem?> Validate(UpdateRewardDefinitionCommand cmd, CancellationToken ct = default)
    {
        if (cmd.Code != null)
        {
            bool isExisted = await rewardRepo.CheckExistedCodeAsync(cmd.Code, ct);
            if (isExisted)
                throw new BadRequestException(MessageCode.Reward.CodeIsAlreadyExisted);
        }

        var catalogItem = cmd.CatalogItemId.HasValue
                        ? await catalogRepo.GetByIdAsync(cmd.CatalogItemId.Value, ct) 
                        : null;

        if (catalogItem is { IsActive: false })
            throw new BadRequestException(MessageCode.Reward.CatalogItemInactive);
        
        return catalogItem;
    }
}