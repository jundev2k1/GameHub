using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.Missions.Create;

public sealed class CreateMissionHandler(
    IUnitOfWork unitOfWork,
    IMissionRepo repo,
    IMissionCacheService cache,
    ILogger<CreateMissionHandler> logger
    ) : ICommandHandler<CreateMissionCommand, Unit>
{
    public async Task<Unit> Handle(CreateMissionCommand request, CancellationToken ct = default)
    {
        var mission = Mission.Create(
            code: request.Code,
            type: request.Type,
            title: request.Title,
            description: request.Description,
            resetType: request.ResetType,
            triggerEvents: request.TriggerEvents,
            configData: request.ConfigData,
            startAt: request.StartAt,
            endAt: request.EndAt
        );

        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                await repo.AddAsync(mission, ct);
                await unitOfWork.SaveChangesAsync(ct);
                await cache.RefreshCache(ct);
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