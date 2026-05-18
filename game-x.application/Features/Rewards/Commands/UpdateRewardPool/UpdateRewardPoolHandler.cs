using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.UpdateRewardPool;

public sealed class UpdateRewardPoolHandler(
    IUnitOfWork unitOfWork,
    IRewardPoolRepo repo,
    IRewardPoolCacheService cache,
    ILogger<UpdateRewardPoolHandler> logger
    ) : ICommandHandler<UpdateRewardPoolCommand, Unit>
{
    public async Task<Unit> Handle(UpdateRewardPoolCommand cmd, CancellationToken ct = default)
    {
        await Validate(cmd, ct);
        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                await repo.UpdateAsync(cmd.Id, x =>
                {
                    x.OnUpdate(
                        code: cmd.Code, 
                        title: cmd.Title, 
                        description: cmd.Description,
                        type: cmd.Type,
                        isActive: cmd.IsActive,
                        sortOrder: cmd.SortOrder,
                        startAt: cmd.StartAt,
                        endAt: cmd.EndAt,
                        config: cmd.Config);
                }, ct);
                await unitOfWork.CommitAsync(ct);
                await cache.RefreshCache(ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to update reward pool.");
                throw new BadRequestException("Failed to update reward pool.");
            }
        }, ct);
        
        return Unit.Value;
    }

    private async Task Validate(UpdateRewardPoolCommand cmd, CancellationToken ct = default)
    {
        if (cmd.Code != null)
        {
            bool isExisted = await repo.CheckExistedCodeAsync(cmd.Code, ct);
            if (isExisted)
                throw new BadRequestException(MessageCode.Reward.CodeIsAlreadyExisted);
        }
    }
}