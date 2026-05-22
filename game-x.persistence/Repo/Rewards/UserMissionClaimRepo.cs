using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Exceptions;
using game_x.domain.Constants;
using game_x.domain.Entities.Rewards;

namespace game_x.persistence.Repo.Rewards;

public sealed class UserMissionClaimRepo(GameXContext dbContext) : IUserMissionClaimRepo, IRepository
{
    public async Task<bool> ExistsAsync(string userId, int missionRewardId, CancellationToken ct = default)
    {
        return await dbContext.UserMissionClaims
            .AnyAsync(x => x.UserId ==  userId && x.MissionRewardId == missionRewardId, ct);
    }
    
    public async Task<UserMissionClaim> GetTrackedByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.UserMissionClaims
            .Include(x => x.MissionReward)
                .ThenInclude(x => x!.RewardDefinition)
            .FirstOrDefaultAsync(x => x.PublicId == id, ct)
            ?? throw new NotFoundException(MessageCode.Reward.MissionClaimNotFound);
    }
    
    public async Task AddAsync(UserMissionClaim entity, CancellationToken ct = default)
    {
        await dbContext.UserMissionClaims.AddAsync(entity, ct);
    }
}