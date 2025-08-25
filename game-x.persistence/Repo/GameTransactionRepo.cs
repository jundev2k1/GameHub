using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.domain.Constants;

namespace game_x.persistence.Repo;

public sealed class GameTransactionRepo(GameXContext context) : IGameTransactionRepo, IRepository
{
    public async Task<PaginationResult<GameTransaction>> GetMyTransactionsAsync(
        string userId,
        Func<IQueryable<GameTransaction>, IQueryable<GameTransaction>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.GameTransactions
            .AsNoTracking()
            .Include(x => x.CryptoToken)
            .Include(x => x.Ledger)
            .Where(x => x.UserId == userId)
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<GameTransaction>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }
    
    public async Task<GameTransaction> GetByIdAndUserIdAsync(string userId, Guid publicId, CancellationToken ct = default)
    {
        return await context.GameTransactions
                   .AsNoTracking()
                   .Include(t => t.User!)
                   .ThenInclude(u => u.UserBalances)
                   .Include(t => t.CryptoToken)
                   .Include(t => t.Ledger)
                   .FirstOrDefaultAsync(x => x.PublicId == publicId && x.UserId == userId, ct)
               ?? throw new NotFoundException(MessageCode.Transaction.TradeNotFound);
    }
    
    public async Task<GameTransaction> AddAsync(GameTransaction entity, CancellationToken ct = default)
    {
        await context.GameTransactions.AddAsync(entity, ct);
        return entity;
    }
    
    public async Task PatchUpdateAsync(Guid publicId, Action<GameTransaction> updateAction, CancellationToken ct = default)
    {
        var transaction = await context.GameTransactions
                              .FirstOrDefaultAsync(c => c.PublicId == publicId, ct)
                               ?? throw new NotFoundException(MessageCode.Transaction.GameTransactionNotFound);

        updateAction.Invoke(transaction);
        await context.SaveChangesAsync(ct);
    }

    public async Task PutUpdateAsync(GameTransaction transaction, CancellationToken ct = default)
    {
        context.Entry(transaction).State = EntityState.Modified;
        await context.SaveChangesAsync(ct);
    }
}
