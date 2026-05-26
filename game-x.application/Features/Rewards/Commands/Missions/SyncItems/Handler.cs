using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.Missions.SyncItems;

public sealed class SyncMissionRewardCommandHandler(
    IUnitOfWork unitOfWork,
    IMissionRepo missionRepo,
    IMissionRewardRepo missionRewardRepo,
    IRewardDefinitionRepo rewardRepo,
    IMissionCacheService cache,
    ILogger<SyncMissionRewardCommandHandler> logger
) : ICommandHandler<SyncMissionRewardCommand, Unit>
{
    public async Task<Unit> Handle(
        SyncMissionRewardCommand cmd,
        CancellationToken ct = default)
    {
        var context = await Validate(cmd, ct);
        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                await DeleteRewards(cmd, ct);
                UpdateRewards(cmd, context);
                await CreateRewards(cmd, context, ct);
                await unitOfWork.CommitAsync(ct);
                await cache.RefreshCache(context.Mission.PublicId, ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to sync mission rewards.");
                throw;
            }
        }, ct);

        return Unit.Value;
    }

    private async Task DeleteRewards(SyncMissionRewardCommand cmd, CancellationToken ct)
    {
        if (cmd.DeletedItems.Count == 0) return;
        await missionRewardRepo.BulkDeleteAsync(x => cmd.DeletedItems.Contains(x.PublicId), ct);
    }

    private void UpdateRewards(
        SyncMissionRewardCommand cmd,
        ValidationContext context)
    {
        foreach (var dto in cmd.UpdatedItems)
        {
            var reward = context.MissionRewardLookup[dto.Id];
            var rewardDefinition = context.RewardLookup[dto.RewardDefinitionId];

            reward.OnUpdate(
                rewardDefinitionId: rewardDefinition.Id,
                sequence: dto.Sequence,
                sortOrder: dto.SortOrder,
                requiredProgress: dto.RequiredProgress,
                isActive: dto.IsActive,
                startAt: dto.StartAt,
                endAt: dto.EndAt);
        }
    }

    private async Task CreateRewards(
        SyncMissionRewardCommand cmd,
        ValidationContext context,
        CancellationToken ct)
    {
        if (cmd.CreatedItems.Count == 0) return;

        var newRewards = cmd.CreatedItems
            .Select(dto =>
            {
                var rewardDefinition = context.RewardLookup[dto.RewardDefinitionId];

                return MissionReward.Create(
                    missionId: context.Mission.Id,
                    rewardDefinitionId: rewardDefinition.Id,
                    sequence: dto.Sequence,
                    sortOrder: dto.SortOrder,
                    requiredProgress: dto.RequiredProgress,
                    startAt: dto.StartAt,
                    endAt: dto.EndAt);
            })
            .ToList();

        await missionRewardRepo.AddRangeAsync(newRewards, ct);
    }

    private async Task<ValidationContext> Validate(
        SyncMissionRewardCommand cmd,
        CancellationToken ct)
    {
        var mission = await missionRepo.GetByIdAsync(cmd.MissionId, ct);
        if (!mission.IsActive)
            throw new BadRequestException(MessageCode.Reward.MissionInactive);

        var rewardDefinitionIds = cmd.CreatedItems
            .Select(x => x.RewardDefinitionId)
            .Concat(cmd.UpdatedItems.Select(x => x.RewardDefinitionId))
            .Distinct()
            .ToHashSet();

        var rewardDefinitions = rewardDefinitionIds.Count == 0
            ? []
            : await rewardRepo.GetByIdsAsync(rewardDefinitionIds, ct);

        var rewardLookup = rewardDefinitions.ToDictionary(x => x.PublicId);

        var missingRewardIds = rewardDefinitionIds
            .Where(id => !rewardLookup.ContainsKey(id))
            .ToList();

        if (missingRewardIds.Count > 0)
            throw new NotFoundException(MessageCode.Reward.RewardDefinitionNotFound);

        var inactiveReward = rewardDefinitions.FirstOrDefault(x => !x.IsActive);

        if (inactiveReward is not null)
            throw new BadRequestException(MessageCode.Reward.RewardDefinitionInactive);

        var missionRewardIds = cmd.UpdatedItems
            .Select(x => x.Id)
            .Concat(cmd.DeletedItems)
            .Distinct()
            .ToHashSet();

        var missionRewards = missionRewardIds.Count == 0
            ? []
            : await missionRewardRepo.GetByIdsForUpdateAsync(missionRewardIds, ct);

        var missionRewardLookup = missionRewards.ToDictionary(x => x.PublicId);
        var missingMissionRewardIds = missionRewardIds
            .Where(id => !missionRewardLookup.ContainsKey(id))
            .ToList();

        if (missingMissionRewardIds.Count > 0)
            throw new NotFoundException(
                MessageCode.Reward.MissionRewardNotFound, 
                new { MissingIds = missingMissionRewardIds });

        var invalidMissionReward = missionRewards
            .Where(x => x.MissionId != mission.Id)
            .ToList();
        
        if(invalidMissionReward.Count > 0)
            throw new BadRequestException(
                MessageCode.Reward.MissionRewardInvalid,
                new { InvalidIds = invalidMissionReward.Select(x => x.PublicId) });

        return new ValidationContext(mission, rewardLookup, missionRewardLookup);
    }

    private sealed record ValidationContext(
        Mission Mission,
        Dictionary<Guid, RewardDefinition> RewardLookup,
        Dictionary<Guid, MissionReward> MissionRewardLookup);
}