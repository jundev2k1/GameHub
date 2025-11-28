using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class SystemWalletRepo(GameXContext dbContext) : ISystemWalletRepo, IRepository
{
    public async Task<PaginationResult<SystemWalletTransaction>> GetsByCriteriaAsync(
        Func<IQueryable<SystemWalletTransaction>, IQueryable<SystemWalletTransaction>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = dbContext.SystemWalletTransactions
            .AsNoTracking()
            .Include(swt => swt.Wallet)
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var totalPageCount = (int)Math.Ceiling((decimal)totalCount / pageSize);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(ct);

        return new PaginationResult<SystemWalletTransaction>(
            items,
            totalCount,
            totalPageCount,
            page,
            pageSize);
    }

    public async Task<SystemWallet[]> GetAllAsync(CancellationToken ct = default)
    {
        return await dbContext.SystemWallets
            .AsNoTracking()
            .ToArrayAsync(ct);
    }

    public async Task<SystemWallet> GetWalletAsync(SystemWalletType type, CancellationToken ct = default)
    {
        return await dbContext.SystemWallets.AsNoTracking().FirstOrDefaultAsync(sw => sw.Type == type, ct)
            ?? throw new NotFoundException(nameof(type), type.ToString());
    }

    public async Task UpdateAsync(SystemWalletType type, Action<SystemWallet> updateAction, CancellationToken ct = default)
    {
        var targetWallet = await dbContext.SystemWallets.FirstOrDefaultAsync(sw => sw.Type == type, ct)
            ?? throw new NotFoundException(nameof(type), type.ToString());

        updateAction.Invoke(targetWallet);
    }
}
