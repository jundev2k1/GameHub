using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;

namespace game_x.persistence.Repo.Rewards;

public sealed class ShareLinkRepo(GameXContext dbContext) : IShareLinkRepo, IRepository
{
    public async Task<ShareLink?> GetActiveByUserAndMissionAsync(string userId, Guid missionId, CancellationToken ct)
    {
        return await dbContext.ShareLinks
            .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.Mission != null &&
                    x.Mission.PublicId == missionId &&
                    x.Status == ShareLinkStatus.Active,
                ct);
    }

    public async Task<ShareLink?> GetByCodeAsync(string code, CancellationToken ct)
    {
        return await dbContext.ShareLinks
            .FirstOrDefaultAsync(x => x.Code == code, ct);
    }

    public async Task AddAsync(ShareLink entity, CancellationToken ct)
    {
        await dbContext.ShareLinks.AddAsync(entity, ct);
    }

    // concurrency-safe
    public async Task IncrementClickAsync(int shareLinkId, CancellationToken ct)
    {
        await dbContext.Database.ExecuteSqlInterpolatedAsync($@"
            UPDATE share_links
            SET click_count = click_count + 1
            WHERE id = {shareLinkId}
        ", ct);
    }
}