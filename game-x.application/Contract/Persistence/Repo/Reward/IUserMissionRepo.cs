using game_x.domain.Entities.Rewards;

namespace game_x.application.Contract.Persistence.Repo.Reward;

public interface IUserMissionRepo
{
    Task<UserMission?> GetByUserAndMissionAsync(string userId, int missionId, CancellationToken ct = default);

    Task AddAsync(UserMission entity, CancellationToken ct = default);
}