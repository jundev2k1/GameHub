using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Events.Rewards.OnUserInventoryUpdated;
using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;

namespace game_x.application.Features.Rewards.Commands.Missions.Claim;

public sealed class ClaimMissionRewardHandler(
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    IUserMissionClaimRepo userMissionClaimRepo,
    IExecutionRepo executionRepo,
    IUserRewardRepo userRewardRepo,
    IUserInventoryRepo inventoryRepo,
    IUserInventoryCacheService userInventoryCache,
    IApplicationEventDispatcher dispatcher
    // IWalletService walletService,
    // IIdempotencyKeyRepo idempotencyRepo
) : ICommandHandler<ClaimMissionRewardCommand, ClaimMissionRewardResponse>
{
    public async Task<ClaimMissionRewardResponse> Handle(
        ClaimMissionRewardCommand cmd,
        CancellationToken ct)
    {
        var userId = userAccessor.GetUserId();

        var claim = await ValidateAsync(userId, cmd.ClaimId, ct);

        var reward = claim.MissionReward!.RewardDefinition!;
        var userMission = claim.UserMission!;
        var mission = userMission.Mission!;

        ClaimMissionRewardResponse response = null!;

        await unitOfWork.WithTransactionAsync(async () =>
        {
            var execution = Execution.Create(
                userId: userId,
                type: ExecutionType.MissionRewardClaim,
                missionId: mission.Id
                // idempotencyKey: cmd.IdempotencyKey
                );

            await executionRepo.AddAsync(execution, ct);

            try
            {
                await GrantRewardAsync(userId, reward, ct);

                var userReward = UserReward.Create(
                    userId: userId,
                    execution: execution,
                    rewardDefinitionId: reward.Id,
                    rewardPoolItemId: null,
                    rewardType: reward.Type,
                    amount: reward.Amount ?? 0,
                    title: reward.Title,
                    catalogItemId: reward.CatalogItemId);

                await userRewardRepo.AddAsync(userReward, ct);

                claim.Claim(execution);

                execution.MarkSuccess();

                var hasPendingClaims = await userMissionClaimRepo.HasPendingClaimsAsync(userMission.Id, userMission.CycleNumber, ct);
                if (!hasPendingClaims && userMission.Status == UserMissionStatus.Completed)
                {
                    await userMissionClaimRepo.ExpireUnclaimedAsync(userMission.Id, userMission.CycleNumber, ct);
                    userMission.ResetProgress();
                }

                await unitOfWork.CommitAsync(ct);

                await userInventoryCache.RefreshCache(userId, ct);
                var inventories = await userInventoryCache.GetAll(userId, ct);
                await dispatcher.Publish(new OnUserInventoryUpdatedEvent(userId, inventories ?? []), ct);

                response = new ClaimMissionRewardResponse
                {
                    RewardId = reward.PublicId,
                    RewardCode = reward.Code,
                    RewardTitle = reward.Title,
                    Amount = reward.Amount ?? 0,
                    RewardType = reward.Type,
                    CycleCompleted = userMission is {Status: UserMissionStatus.InProgress, Progress: 0}
                };
            }
            catch (Exception ex)
            {
                execution.MarkFailed(ex.Message);
                throw;
            }
        }, ct);

        return response;
    }

    private async Task<UserMissionClaim> ValidateAsync(string userId, Guid claimId, CancellationToken ct)
    {
        var claim = await userMissionClaimRepo.GetTrackedByIdAsync(claimId, ct);

        if (claim is null)
            throw new NotFoundException(MessageCode.Reward.MissionClaimNotFound);

        if (claim.UserId != userId)
            throw new NotFoundException(MessageCode.Reward.MissionClaimInvalid);

        if (claim.Status != UserMissionClaimStatus.Available)
            throw new BadRequestException(MessageCode.Reward.MissionClaimUnavailable);

        if (claim.MissionReward is null)
            throw new NotFoundException(MessageCode.Reward.MissionRewardNotFound);

        if (claim.UserMission is null)
            throw new NotFoundException(MessageCode.Reward.UserMissionNotFound);

        if (claim.UserMission.Mission is null)
            throw new NotFoundException(MessageCode.Reward.MissionNotFound);

        if (claim.MissionReward.RewardDefinition is null)
            throw new NotFoundException(MessageCode.Reward.RewardDefinitionNotFound);

        if (!claim.MissionReward.RewardDefinition.CatalogItemId.HasValue) 
            throw new BadRequestException(MessageCode.Reward.CatalogNotFound);
        
        return claim;
    }

    private async Task GrantRewardAsync(string userId, RewardDefinition reward, CancellationToken ct)
    {
        switch (reward.Type)
        {
            case RewardItemType.CatalogItem:
            {
                var amount = (int)(reward.Amount ?? 0);
                if (amount <= 0)
                    throw new BadRequestException(MessageCode.Reward.RewardDefinitionAmountInvalid);

                var inventory = await inventoryRepo.GetDetailAsync(userId, reward.CatalogItemId!.Value, ct);
                if (inventory is null)
                {
                    inventory = UserInventory.Create(userId, reward.CatalogItemId.Value, amount);
                    await inventoryRepo.AddAsync(inventory, ct);
                }
                else
                    inventory.Add(amount);
                break;
            }

            /*
            case RewardItemType.Currency:
            {
                await walletService.CreditAsync(
                    userId,
                    reward.Amount ?? 0,
                    ct);
                break;
            }
            */

            default:
                throw new BadRequestException(MessageCode.Reward.RewardDefinitionUnsupportedType);
        }
    }
}