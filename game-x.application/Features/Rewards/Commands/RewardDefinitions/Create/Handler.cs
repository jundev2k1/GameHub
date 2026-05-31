using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.RewardDefinitions.Create;

public sealed class CreateRewardDefinitionHandler(
    IUnitOfWork unitOfWork,
    IRewardDefinitionRepo rewardRepo,
    ICatalogItemRepo catalogRepo,
    IRewardDefinitionCacheService rewardCache,
    ILogger<CreateRewardDefinitionHandler> logger
    ) : ICommandHandler<CreateRewardDefinitionCommand, Unit>
{
    public async Task<Unit> Handle(CreateRewardDefinitionCommand request, CancellationToken ct = default)
    {
        var isExisted = await rewardRepo.CheckExistedCodeAsync(request.Code, ct);
        if (isExisted)
            throw new BadRequestException(MessageCode.Reward.CodeIsAlreadyExisted);
        
        var reward = RewardDefinition.Create(
            code: request.Code,
            title: request.Title,
            type: request.Type,
            amount: request.Amount,
            description: request.Description,
            metadata: request.Metadata
        );
 
        if (request.CatalogItemId.HasValue)
        {
            var catalogItem = await catalogRepo.GetByIdAsync(request.CatalogItemId.Value, ct);
            if (!catalogItem.IsActive)
                throw new BadRequestException(MessageCode.Reward.CatalogItemInactive);
            reward.AddCatalog(catalogItem.Id);
        }

        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                await rewardRepo.AddAsync(reward, ct);
                await unitOfWork.SaveChangesAsync(ct);
                await rewardCache.RefreshCache(ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to create reward definition.");
                throw new BadRequestException("Failed to create reward definition.", e);
            }
        }, ct);
        
        return Unit.Value;
    }
}