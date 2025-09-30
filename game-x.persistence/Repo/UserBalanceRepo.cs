using EFCore.BulkExtensions;
using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class UserBalanceRepo(GameXContext context) : IUserBalanceRepo, IRepository
{
    public async Task<IEnumerable<UserBalance>> GetBalancesByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return await context.UserBalances
            .AsNoTracking()
            .Include(item => item.CryptoToken)
            .Where(x => x.UserId == userId)
            .ToListAsync(ct);
    }

    public async Task<UserBalance?> GetByUserIdAndTokenIdAsync(string userId, int cryptoTokenId, CancellationToken ct = default)
    {
        return await context.UserBalances
            .Include(x => x.CryptoToken)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.CryptoTokenId == cryptoTokenId, ct);
    }
    public async Task<UserBalance> GetByUserIdAndTokenIdAsync(string userId, Guid cryptoTokenId, CancellationToken ct = default)
    {
        return await context.UserBalances
            .Include(x => x.CryptoToken)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.CryptoToken.PublicId == cryptoTokenId, ct)
            ?? throw new NotFoundException(nameof(cryptoTokenId), cryptoTokenId);
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

    public async Task UpdateAsync(Guid id, Action<UserBalance> updateAction, CancellationToken ct = default)
    {
        var targetBalance = await context.UserBalances
            .FirstOrDefaultAsync(x => x.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        updateAction.Invoke(targetBalance);
    }
    public async Task UpdateAsync(Guid id, Func<UserBalance, Task> updateAction, CancellationToken ct = default)
    {
        var targetBalance = await context.UserBalances
            .FirstOrDefaultAsync(x => x.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        await updateAction.Invoke(targetBalance);
    }

    public async Task PutUpdateAsync(UserBalance ub, CancellationToken ct = default)
    {
        context.Entry(ub).State = EntityState.Modified;
        await context.SaveChangesAsync(ct);
    }
}
