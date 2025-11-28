using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using Polly;

namespace game_x.persistence.Repo;

public sealed class TalentWalletRepo(GameXContext dbContext) : ITalentWalletRepo, IRepository
{
    public async Task<PaginationResult<TalentWalletTransaction>> GetsByCriteriaAsync(
        Func<IQueryable<TalentWalletTransaction>, IQueryable<TalentWalletTransaction>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = dbContext.TalentWalletTransactions
            .AsNoTracking()
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var totalPageCount = (int)Math.Ceiling((decimal)totalCount / pageSize);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(ct);

        return new PaginationResult<TalentWalletTransaction>(
            items,
            totalCount,
            totalPageCount,
            page,
            pageSize);
    }

    public async Task<TalentWallet> GetWalletAsync(string userId, CancellationToken ct = default)
    {
        return await dbContext.TalentWallets.AsNoTracking().FirstOrDefaultAsync(tw => tw.Id == userId, ct)
            ?? throw new NotFoundException(nameof(userId), userId);
    }

    public async Task UpdateAsync(string userId, Action<TalentWallet> updateAction, CancellationToken ct = default)
    {
        var target = await dbContext.TalentWallets
            .FirstOrDefaultAsync(tw => tw.Id == userId, ct)
            ?? throw new NotFoundException(nameof(userId), userId);

        updateAction.Invoke(target);
    }
}
