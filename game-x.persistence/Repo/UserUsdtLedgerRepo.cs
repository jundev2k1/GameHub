using game_x.application.Contract.Persistence.Repo;

namespace game_x.persistence.Repo;

public sealed class UserUsdtLedgerRepo(GameXContext context): IUserUsdtLedgerRepo
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
}