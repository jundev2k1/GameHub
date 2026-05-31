using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class GamePlatformBalanceRepo(
    GameXContext dbContext,
    IUnitOfWork unitOfWork) : IGamePlatformBalanceRepo, IRepository
{
    public async Task<IEnumerable<GamePlatformBalance>> GetBalancesByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await dbContext.GamePlatformBalances
            .AsNoTracking()
            .Include(item => item.Platform)
            .Where(x => x.UserId == userId)
            .ToArrayAsync(ct);
    }

    public async Task<GamePlatformBalance?> GetByPlatformIdAsync(string userId, Guid platformId, CancellationToken ct = default)
    {
        return await dbContext.GamePlatformBalances
            .AsNoTracking()
            .Include(x => x.Platform)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Platform.PublicId == platformId, ct)
            ?? throw new NotFoundException($"GamePlatformBalance ({userId} | {platformId}) was not found.");
    }

    public async Task SyncOrCreateAsync(
        string userId,
        int platformId,
        decimal availableBalance,
        decimal lockedBalance,
        CancellationToken ct = default)
    {
        var balance = await dbContext.GamePlatformBalances
            .FirstOrDefaultAsync(gpb => gpb.UserId == userId && gpb.PlatformId == platformId, ct);
        if (balance == null)
        {
            var newBalance = GamePlatformBalance.Create(userId, platformId);
            newBalance.SyncBalance(availableBalance, lockedBalance);
            await dbContext.GamePlatformBalances.AddAsync(newBalance, ct);
            return;
        }

        balance.SyncBalance(availableBalance, lockedBalance);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
