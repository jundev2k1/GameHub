using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;

namespace game_x.application.Features.Rewards.Commands.Missions.Claim;

public sealed class ClaimMissionRewardHandler(
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    IMissionRepo missionRepo,
    IUserMissionClaimRepo userMissionClaimRepo,
    IExecutionRepo executionRepo,
    IUserRewardRepo userRewardRepo,
    IUserInventoryRepo inventoryRepo
    // IWalletService walletService,
    // IIdempotencyKeyRepo idempotencyRepo
) : ICommandHandler<ClaimMissionRewardCommand, ClaimMissionRewardResponse>
{
    public async Task<ClaimMissionRewardResponse> Handle(ClaimMissionRewardCommand cmd, CancellationToken ct)
    {
        var userId = userAccessor.GetUserId();
        var (mission, reward, claim) = await Validate(userId, cmd, ct);
        ClaimMissionRewardResponse response = new();
        await unitOfWork.WithTransactionAsync(async () =>
        {
            // // 1. Idempotency
            // if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
            // {
            //     var existing =
            //         await idempotencyRepo.GetAsync(
            //             userId,
            //             request.IdempotencyKey,
            //             ct);
            //
            //     if (existing is not null &&
            //         !existing.IsExpired() &&
            //         !string.IsNullOrWhiteSpace(existing.ResponseMetadata))
            //     {
            //         return JsonSerializer.Deserialize<ClaimMissionRewardResponse>(
            //             existing.ResponseMetadata)!;
            //     }
            // }
            
            // 4. Create execution
            var execution = Execution.Create(
                userId: userId,
                type: ExecutionType.MissionRewardClaim,
                missionId: mission.Id
                // idempotencyKey: cmd.IdempotencyKey
                );
            await executionRepo.AddAsync(execution, ct);

            try
            {
                // 5. Grant reward
                await GrantRewardAsync(userId, reward, ct);

                // 6. Create ledger
                var userReward = UserReward.Create(
                    userId: userId,
                    executionId: execution.Id,
                    rewardDefinitionId: reward.Id,
                    rewardPoolItemId: null,
                    rewardType: reward.Type,
                    amount: reward.Amount ?? 0,
                    title: reward.Title,
                    catalogItemId: reward.CatalogItemId
                );

                await userRewardRepo.AddAsync(userReward, ct);

                // 7. mark claim
                claim.Claim(execution);

                execution.MarkSuccess();

                await unitOfWork.CommitAsync(ct);
                
                response = new ClaimMissionRewardResponse
                {
                    ExecutionId = execution.PublicId,
                    UserRewardId = userReward.PublicId,
                    RewardTitle = reward.Title,
                    Amount = reward.Amount ?? 0,
                    RewardType = reward.Type
                };
                    
                // 8. save idempotency response
                // if (!string.IsNullOrWhiteSpace(request.IdempotencyKey))
                // {
                //     var key = IdempotencyKey.Create(
                //         key: request.IdempotencyKey,
                //         userId: userId,
                //         actionType: IdempotencyActionType.MissionClaim,
                //         responseMetadata: JsonSerializer.Serialize(response),
                //         expiredAt: DateTime.UtcNow.AddHours(24));
                //
                //     await idempotencyRepo.AddAsync(key, ct);
                // }
            }
            catch (Exception ex)
            {
                execution.MarkFailed(ex.Message);
                throw;
            }
        }, ct);
        
        return response;
    }

    private async Task<(Mission mission, RewardDefinition reward, UserMissionClaim claim)> 
        Validate(string userId, ClaimMissionRewardCommand request, CancellationToken ct = default)
    {
        var mission = await missionRepo.GetByIdAsync(request.MissionId, ct);
        
        var claim = await userMissionClaimRepo.GetByIdAsync(request.ClaimId, ct);
        if (claim.MissionReward is null)
            throw new NotFoundException(MessageCode.Reward.MissionRewardNotFound);
            
        if (claim.UserId != userId || claim.MissionReward?.MissionId != mission.Id)
            throw new NotFoundException(MessageCode.Reward.MissionClaimInvalid);

        if (claim.Status != UserMissionClaimStatus.Available)
            throw new BadRequestException(MessageCode.Reward.MissionClaimUnavailable);
        
        var rewardDefinition = claim.MissionReward.RewardDefinition;
        if (rewardDefinition is null)
            throw new NotFoundException(MessageCode.Reward.RewardDefinitionNotFound);
        
        if (!rewardDefinition.CatalogItemId.HasValue) 
            throw new BadRequestException(MessageCode.Reward.CatalogNotFound);
        
        return (mission, rewardDefinition, claim);
    }
    
    private async Task GrantRewardAsync(string userId, RewardDefinition reward, CancellationToken ct = default)
    {
        switch (reward.Type)
        {
            // case RewardItemType.Currency:
            // {
            //     await walletService.CreditAsync(
            //         userId,
            //         reward.Amount,
            //         ct);
            //
            //     break;
            // }

            case RewardItemType.CatalogItem:
            {
                if (!reward.CatalogItemId.HasValue) throw new BadRequestException(MessageCode.Reward.CatalogNotFound);

                var inventory = await inventoryRepo.GetDetailAsync(userId, reward.CatalogItemId.Value, ct);
                if (inventory is null)
                {
                    inventory = UserInventory.Create(userId, reward.CatalogItemId.Value, (int?)reward.Amount ?? 0);
                    await inventoryRepo.AddAsync(inventory, ct);
                }
                else
                    inventory.Add((int?)reward.Amount ?? 0);

                break;
            }

            default:
                throw new BadRequestException(MessageCode.Reward.RewardDefinitionUnsupportedType);
        }
    }
}