using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Exceptions;
using game_x.domain.Constants;
using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;

namespace game_x.persistence.Repo.Rewards;

public sealed class UserMissionClaimRepo(GameXContext dbContext) : IUserMissionClaimRepo, IRepository
{
    public async Task<bool> ExistsAsync(
        int userMissionId, 
        int missionRewardId, 
        int cycleNumber, 
        CancellationToken ct = default)
    {
        return await dbContext.UserMissionClaims
            .AnyAsync(x => 
                x.UserMissionId == userMissionId && 
                x.MissionRewardId == missionRewardId && 
                x.CycleNumber == cycleNumber &&
                x.Status != UserMissionClaimStatus.Expired, ct);
    }

    public async Task ExpireUnclaimedAsync(
        int userMissionId,
        int cycleNumber,
        CancellationToken ct = default)
    {
        await dbContext.UserMissionClaims
            .Where(x =>
                x.UserMissionId == userMissionId &&
                x.CycleNumber == cycleNumber &&
                x.Status == UserMissionClaimStatus.Available)
            .ExecuteUpdateAsync(
                setter => setter
                    .SetProperty(x => x.Status, UserMissionClaimStatus.Expired)
                    .SetProperty(x => x.ExpiredAt, DateTime.UtcNow),
                ct);
    }
    
    public async Task<UserMissionClaim> GetTrackedByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.UserMissionClaims
            .Include(x => x.UserMission)
                .ThenInclude(x => x!.Mission)
            .Include(x => x.MissionReward)
                .ThenInclude(x => x!.RewardDefinition)
            .FirstOrDefaultAsync(x => x.PublicId == id, ct)
            ?? throw new NotFoundException(MessageCode.Reward.MissionClaimNotFound);
    }
    
    public async Task<bool> HasPendingClaimsAsync(int userMissionId, int cycleNumber, CancellationToken ct = default)
    {
        return await dbContext.UserMissionClaims
            .AnyAsync(x =>
                    x.UserMissionId == userMissionId &&
                    x.CycleNumber == cycleNumber &&
                    x.Status == UserMissionClaimStatus.Available,
                ct);
    }
    
    public async Task AddAsync(UserMissionClaim entity, CancellationToken ct = default)
    {
        await dbContext.UserMissionClaims.AddAsync(entity, ct);
    }
}