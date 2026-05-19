using System.Diagnostics;
using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.MissionRewards.Create;

public sealed class CreateMissionRewardHandler(
    IUnitOfWork unitOfWork,
    IMissionRepo missionRepo,
    IRewardDefinitionRepo rewardRepo,
    IMissionRewardRepo missionRewardRepo,
    IMissionCacheService cache,
    ILogger<CreateMissionRewardHandler> logger) : ICommandHandler<CreateMissionRewardCommand, Unit>
{
    public async Task<Unit> Handle(CreateMissionRewardCommand cmd, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        var (mission, reward) = await Validate(cmd, ct);
        var missionReward = MissionReward.Create(
            missionId: mission.Id,
            rewardDefinitionId: reward.Id,
            sequence: cmd.Sequence,
            sortOrder: cmd.SortOrder,
            requiredProgress: cmd.RequiredProgress,
            isClaimable: cmd.IsClaimable,
            startAt: cmd.StartAt,
            endAt: cmd.EndAt
        );

        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                await missionRewardRepo.AddAsync(missionReward, ct);
                await unitOfWork.SaveChangesAsync(ct);
                logger.LogInformation("Mission query took {Ms} ms", sw.ElapsedMilliseconds);
                await cache.RefreshCache(mission.PublicId, ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to create mission reward.");
                throw new BadRequestException("Failed to create mission reward.", e);
            }
        }, ct);
        logger.LogInformation("Finish {Ms} ms", sw.ElapsedMilliseconds);
        sw.Stop();
        return Unit.Value;
    }

    private async Task<(Mission mission, RewardDefinition reward)> Validate(CreateMissionRewardCommand cmd, CancellationToken ct = default)
    {
        var mission = await missionRepo.GetByIdAsync(cmd.MissionId, ct);
        if (!mission.IsActive)
            throw new BadRequestException(MessageCode.Reward.MissionInactive);

        var reward = await rewardRepo.GetByIdAsync(cmd.RewardDefinitionId, ct);
        if(!reward.IsActive)
            throw new BadRequestException(MessageCode.Reward.RewardDefinitionInactive);
        
        return (mission, reward);
    }
}