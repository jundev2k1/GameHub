using game_x.domain.Entities.Rewards;

namespace game_x.application.Contract.Persistence.Repo.Reward;

public interface IShareLinkRepo
{
    Task<ShareLink?> GetActiveByUserAndMissionAsync(string userId, Guid missionId, CancellationToken ct);

    Task<ShareLink?> GetByCodeAsync(string code, CancellationToken ct);

    Task AddAsync(ShareLink entity, CancellationToken ct);

    Task IncrementClickAsync(int shareLinkId, CancellationToken ct);
}