using game_x.domain.Entities.Rewards;

namespace game_x.application.Contract.Persistence.Repo.Reward;

public interface IUserMissionClaimRepo
{
    Task<bool> ExistsAsync(
        int userMissionId, 
        int missionRewardId, 
        int cycleNumber, 
        CancellationToken ct = default);
    
    Task ExpireUnclaimedAsync(int userMissionId, int cycleNumber, CancellationToken ct = default);
    
    Task<UserMissionClaim> GetTrackedByIdAsync(Guid id, CancellationToken ct = default);
    
    Task AddAsync(UserMissionClaim entity, CancellationToken ct = default);
}