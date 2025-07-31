using game_x.application.Contract.Persistence.Repo;

namespace game_x.persistence.Repo;

public sealed class UserBalanceRepo(GameXContext context): IUserBalanceRepo
{
    public IQueryable<UserBalance> Query()
    {
        return context.UserBalances;
    }

    public async Task<UserBalance?> GetByUserIdAndTokenIdAsync(string userId, int cryptoTokenId, CancellationToken ct = default)
    {
        return await context.UserBalances
            .FirstOrDefaultAsync(x => x.UserId == userId && x.CryptoTokenId == cryptoTokenId, ct);
    }

    public async Task<(decimal totalUserAmount, decimal totalUserForzenAmount)> GetTotalUserAndAgentAvailableBalanceAsync(CancellationToken ct)
    {
        var allUsers = await context.Users
            .Include(u => u.UserBalances)
            .Include(u => u.UserRoles)
            .ThenInclude(r => r.Role)
            .Where(u => u.IsUser)
            .ToListAsync(ct);

        var userList = allUsers.Where(u => u.IsUser).ToList();

        var userAmount = userList.SelectMany(u => u.UserBalances).Sum(b => b.Amount);

        var userFrozenAmount = userList.SelectMany(u => u.UserBalances).Sum(b => b.FrozenAmount);

        return (userAmount,  userFrozenAmount);
    }
}