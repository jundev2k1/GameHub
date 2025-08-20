using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.domain.Constants;

namespace game_x.persistence.Repo;

public sealed class ChainTransactionRepo(GameXContext context) : IChainTransactionRepo, IRepository
{
    public IQueryable<ChainTransaction> Query()
    {
        return context.ChainTransactions;
    }

    public async Task<PaginationResult<ChainTransaction>> GetTransactionByCriteriaAsync(
        Func<IQueryable<ChainTransaction>, IQueryable<ChainTransaction>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.ChainTransactions
            .AsNoTracking()
            .Include(x => x.CryptoToken)
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<ChainTransaction>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }

    public async Task<PaginationResult<ChainTransaction>> GetMyTransactionsAsync(
        string userId,
        Func<IQueryable<ChainTransaction>, IQueryable<ChainTransaction>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.ChainTransactions
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

        return new PaginationResult<ChainTransaction>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }
    
    public async Task<PaginationResult<ChainTransaction>> GetOngoingTransactionCriteriaByUserAsync(
        string userId,
        Func<IQueryable<ChainTransaction>, IQueryable<ChainTransaction>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.ChainTransactions
            .AsNoTracking()
            .Include(x => x.CryptoToken)
            .Where(x => x.UserId == userId && x.Status == ChainTransactionStatus.Pending)
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<ChainTransaction>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }
    
    public async Task<ChainTransaction> GetByIdAsync(Guid publicId, CancellationToken ct = default)
    {
        return await context.ChainTransactions
            .AsNoTracking()
            .Include(t => t.User!)
                .ThenInclude(u => u.UserBalances)
            .Include(t => t.CryptoToken)
            .Include(t => t.Ledger)
            .FirstOrDefaultAsync(x => x.PublicId == publicId, ct)
            ?? throw new NotFoundException(MessageCode.Transaction.TradeNotFound);
    }
    
    public async Task<ChainTransaction> GetByIdAndUserIdAsync(string userId, Guid publicId, CancellationToken ct = default)
    {
        return await context.ChainTransactions
                   .AsNoTracking()
                   .Include(t => t.User!)
                   .ThenInclude(u => u.UserBalances)
                   .Include(t => t.CryptoToken)
                   .Include(t => t.Ledger)
                   .FirstOrDefaultAsync(x => x.PublicId == publicId && x.UserId == userId, ct)
               ?? throw new NotFoundException(MessageCode.Transaction.TradeNotFound);
    }
    
    public async Task<ChainTransaction> GetOngoingTransactionDetailByUserAsync(string userId, Guid publicId, CancellationToken ct = default)
    {
        return await context.ChainTransactions
            .AsNoTracking()
            .Include(t => t.User!)
                .ThenInclude(u => u.UserBalances)
            .Include(t => t.CryptoToken)
            .FirstOrDefaultAsync(x => x.PublicId == publicId && x.UserId == userId && x.Status == ChainTransactionStatus.Pending, ct)
            ?? throw new NotFoundException(MessageCode.Transaction.TradeNotFound);
    }

    public async Task<bool> ExistsByOrderNoAsync(string otcOrderNo, CancellationToken ct)
    {
        return await context.ChainTransactions.AnyAsync(cl => cl.OrderNumber == otcOrderNo, ct);
    }

    public async Task<ChainTransaction?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct)
    {
        return await context.ChainTransactions
            .Include(t => t.User!)
                .ThenInclude(u => u.UserBalances)
            .Include(t => t.CryptoToken)
            .Include(t => t.Ledger)
            .FirstOrDefaultAsync(x => x.OrderNumber == orderNumber, ct);
    }

    public async Task<(decimal txLogUserFrozenAmount, decimal chainTxLogpendingFee)> GetTxLogSummaryAsync(CancellationToken ct)
    {
        // Only retrieve pending withdrawals
        var pendingWithdrawals = await context.ChainTransactions
            .Where(x => x.Status == ChainTransactionStatus.Pending && x.Type == ChainTransactionType.Withdrawal)
            .Include(x => x.User!)
            .ThenInclude(u => u.UserRoles)
            .ThenInclude(r => r.Role)
            .ToListAsync(ct);

        var confirmedWithdrawals = await context.ChainTransactions
            .Where(x => x.Status == ChainTransactionStatus.Completed && x.Type == ChainTransactionType.Withdrawal)
            .ToListAsync(ct);

        var userAmount = pendingWithdrawals
            .Where(x => x.User is { IsUser: true }).Sum(x => x.Amount);

        // Expected vs. Realized Fees
        var pendingFee = pendingWithdrawals.Sum(x => x.Fee);
        var confirmedFee = confirmedWithdrawals.Sum(x => x.Fee);

        return (userAmount, pendingFee);
    }

    public async Task AddAsync(ChainTransaction transaction, CancellationToken ct = default)
    {
        await context.ChainTransactions.AddAsync(transaction, ct);
    }

    public async Task PatchUpdateAsync(Guid publicId, Action<ChainTransaction> updateAction, CancellationToken ct = default)
    {
        var chainTransaction = await context.ChainTransactions
            .FirstOrDefaultAsync(c => c.PublicId == publicId, ct)
            ?? throw new NotFoundException(MessageCode.Transaction.ChainTransactionNotFound);

        updateAction.Invoke(chainTransaction);
        await context.SaveChangesAsync(ct);
    }

    public async Task PutUpdateAsync(ChainTransaction transaction, CancellationToken ct = default)
    {
        context.Entry(transaction).State = EntityState.Modified;
        await context.SaveChangesAsync(ct);
    }
}
