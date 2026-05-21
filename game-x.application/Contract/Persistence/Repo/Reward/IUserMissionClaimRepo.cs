using game_x.domain.Entities.Rewards;

namespace game_x.application.Contract.Persistence.Repo.Reward;

public interface IUserMissionClaimRepo
{
    Task<bool> CheckExistAsync(string userId, int missionRewardId, CancellationToken ct = default);
    
    Task<UserMissionClaim> GetByIdAsync(Guid id, CancellationToken ct = default);
    
    Task AddAsync(UserMissionClaim entity, CancellationToken ct = default);
}