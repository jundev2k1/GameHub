using game_x.domain.Entities.Rewards;

namespace game_x.application.Contract.Persistence.Repo.Reward;

public interface IMissionRewardRepo
{
    Task<IReadOnlyCollection<MissionReward>> GetByMissionIdAsync(int missionId, CancellationToken ct = default);
    
    Task AddAsync(MissionReward entity, CancellationToken ct = default);
}