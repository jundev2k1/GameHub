using EFCore.BulkExtensions;
using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.domain.Constants;

namespace game_x.persistence.Repo;

public sealed class UserBalanceRepo(GameXContext context): IUserBalanceRepo, IRepository
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
    
    public async Task BulkInsertAsync(IEnumerable<UserBalance>? userBalances)
    {
        var list = userBalances?.ToList();
        if (list is null || list.Count == 0)
            return;

        var config = new BulkConfig
        {
            PreserveInsertOrder = true,
            SetOutputIdentity = true,
            PropertiesToExcludeOnUpdate = [nameof(UserBalance.Version)]
        };

        await context.BulkInsertAsync(list, config);
    }
    
    public async Task CreateAsync(UserBalance userBalance)
    {
        await context.AddAsync(userBalance);
    }
    
    public async Task PatchUpdateAsync(Guid publicId, Action<UserBalance> updateAction, CancellationToken ct = default)
    {
        var userBalance = await context.UserBalances
            .FirstOrDefaultAsync(c => c.PublicId == publicId, ct)
            ?? throw new NotFoundException(MessageCode.Accounting.BalanceNotFound);

        updateAction.Invoke(userBalance);
    }
    
    public async Task PutUpdateAsync(UserBalance ub, CancellationToken ct = default)
    {
        context.Entry(ub).State = EntityState.Modified;
        await context.SaveChangesAsync(ct);
    }
}