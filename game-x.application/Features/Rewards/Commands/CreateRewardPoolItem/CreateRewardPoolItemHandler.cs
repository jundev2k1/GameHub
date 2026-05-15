using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.CreateRewardPoolItem;

public sealed class CreateRewardPoolItemHandler(
    IUnitOfWork unitOfWork,
    IRewardPoolItemRepo itemRepo,
    IRewardPoolRepo poolRepo,
    IRewardDefinitionRepo rewardRepo,
    IRewardPoolItemCacheService cache,
    ILogger<CreateRewardPoolItemHandler> logger
    ) : ICommandHandler<CreateRewardPoolItemCommand, Unit>
{
    public async Task<Unit> Handle(CreateRewardPoolItemCommand request, CancellationToken ct = default)
    {
        var rewardPool = await poolRepo.GetDetailByIdAsync(request.RewardPoolId, ct);
        var rewardDefinition = await rewardRepo.GetDetailByIdAsync(request.RewardDefinitionId, ct);
        
        var item = RewardPoolItem.Create(
            rewardPoolId: rewardPool.Id,
            rewardDefinitionId: rewardDefinition.Id,
            weight: request.Weight,
            sortOrder: request.SortOrder,
            startAt: request.StartAt,
            endAt: request.EndAt
        );
        
        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                await itemRepo.AddAsync(item, ct);
                await unitOfWork.SaveChangesAsync(ct);
                await cache.RefreshCache(rewardPool.PublicId, ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to create mission.");
                throw new BadRequestException("Failed to create mission.", e);
            }
        }, ct);
        
        return Unit.Value;
    }
}