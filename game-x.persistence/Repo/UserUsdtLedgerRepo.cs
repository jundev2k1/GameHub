using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.persistence.Repo;

public sealed class UserUsdtLedgerRepo(GameXContext context): IUserUsdtLedgerRepo, IRepository
{
    public IQueryable<UserUsdtLedger> Query()
    {
        return context.UserUsdtLedgers;
    }

    public async Task<UserUsdtLedger?> GetLatestLedgerAsync(string userId)
    {
        return await context.UserUsdtLedgers
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();
    }
    
    public async Task AddAsync(UserUsdtLedger userUsdtLedger, CancellationToken ct = default)
    {
        await context.UserUsdtLedgers.AddAsync(userUsdtLedger, ct);
        await context.SaveChangesAsync(ct);
    }
}