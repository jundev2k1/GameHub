using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.Missions.Create;

public sealed class CreateMissionHandler(
    IUnitOfWork unitOfWork,
    IMissionRepo repo,
    IMissionCacheService cache,
    ILogger<CreateMissionHandler> logger
    ) : ICommandHandler<CreateMissionCommand, Unit>
{
    public async Task<Unit> Handle(CreateMissionCommand cmd, CancellationToken ct = default)
    {
        await Validate(cmd, ct);
        var mission = Mission.Create(
            code: cmd.Code,
            type: cmd.Type,
            title: cmd.Title,
            description: cmd.Description,
            resetType: cmd.ResetType,
            triggerEvents: GetTriggerEvents(cmd),
            configData: cmd.ConfigData,
            startAt: cmd.StartAt,
            endAt: cmd.EndAt
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
    
    private async Task Validate(CreateMissionCommand cmd, CancellationToken ct = default)
    {
        bool isExisted = await repo.CodeExistsAsync(cmd.Code, ct);
        if (isExisted)
            throw new BadRequestException(MessageCode.Reward.CodeIsAlreadyExisted);
    }

    private UserEventType[] GetTriggerEvents(CreateMissionCommand cmd)
    {
        switch (cmd.Type)
        {
            case MissionType.DailyLogin:
                return [UserEventType.DailyLogin];
            case MissionType.DepositAccumulation:
            case MissionType.Deposit:
                return [UserEventType.DepositCompleted];
            case MissionType.Share:
                return [UserEventType.ShareCompleted];
            default:
                return [];
        }
    }
}