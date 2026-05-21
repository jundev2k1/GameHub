using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;

namespace game_x.persistence.Repo.Rewards;

public sealed class UserMissionClaimRepo(GameXContext dbContext) : IUserMissionClaimRepo, IRepository
{
    public async Task<bool> CheckExistAsync(string userId, int missionRewardId, CancellationToken ct = default)
    {
        return await dbContext.UserMissionClaims
            .AsNoTracking()
            .AnyAsync(x => x.UserId ==  userId && x.MissionRewardId == missionRewardId, ct);
    }
    
    public async Task AddAsync(UserMissionClaim entity, CancellationToken ct = default)
    {
        await dbContext.UserMissionClaims.AddAsync(entity, ct);
    }
}