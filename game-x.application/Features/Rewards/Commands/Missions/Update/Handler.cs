using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.Missions.Update;

public sealed class UpdateMissionValidatorHandler(
    IUnitOfWork unitOfWork,
    IMissionRepo repo,
    IMissionCacheService cache,
    ILogger<UpdateMissionValidatorHandler> logger
    ) : ICommandHandler<UpdateMissionCommand, Unit>
{
    public async Task<Unit> Handle(UpdateMissionCommand cmd, CancellationToken ct = default)
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
                        resetType: cmd.ResetType,
                        triggerEvents: cmd.TriggerEvents,
                        isActive: cmd.IsActive,
                        startAt: cmd.StartAt,
                        endAt: cmd.EndAt,
                        config: cmd.Config);
                }, ct);
    
                await unitOfWork.CommitAsync(ct);
                await cache.RefreshCache(ct);
                await cache.RefreshCache(cmd.Id, ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to update mission.");
                throw;
            }
        }, ct);
        
        return Unit.Value;
    }

    private async Task Validate(UpdateMissionCommand cmd, CancellationToken ct = default)
    {
        if (cmd.Code != null)
        {
            bool isExisted = await repo.CodeExistsAsync(cmd.Code, ct);
            if (isExisted)
                throw new BadRequestException(MessageCode.Reward.CodeIsAlreadyExisted);
        }
    }
}