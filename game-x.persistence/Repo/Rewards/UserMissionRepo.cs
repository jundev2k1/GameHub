using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;

namespace game_x.persistence.Repo.Rewards;

public sealed class UserMissionRepo(GameXContext dbContext) : IUserMissionRepo, IRepository
{
    public async Task<UserMission?> GetByUserAndMissionAsync(string userId, int missionId, CancellationToken ct = default)
    {
        return await dbContext.UserMissions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId ==  userId && x.MissionId == missionId, ct);
    }
    
    public async Task AddAsync(UserMission entity, CancellationToken ct = default)
    {
        await dbContext.UserMissions.AddAsync(entity, ct);
    }
}