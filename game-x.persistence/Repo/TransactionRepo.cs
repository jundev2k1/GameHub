using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.Transactions.Dtos;
using game_x.domain.Constants;
using Mapster;

namespace game_x.persistence.Repo;

public class TransactionRepo(GameXContext context) : ITransactionRepo, IRepository
{
    public async Task<PaginationResult<Transaction>> GetInternalTransactionsAsync(
        Func<IQueryable<Transaction>, IQueryable<Transaction>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.Transactions
            .AsNoTracking()
            .Where(x => x.SourceType == TransactionSourceType.Uxm)
            .Include(x => x.User)
            .Include(x => x.CryptoToken)
            .Include(x => x.TransactionInternal)
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<Transaction>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }

    public async Task<PaginationResult<Transaction>> GetExternalTransactionsAsync(
        Func<IQueryable<Transaction>, IQueryable<Transaction>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.Transactions
            .AsNoTracking()
            .Where(x => x.SourceType != TransactionSourceType.Uxm && x.SourceType != TransactionSourceType.GameX)
            .Include(x => x.User)
            .Include(x => x.CryptoToken)
            .Include(x => x.TransactionExternal)
                .ThenInclude(x => x!.GamePlatform)
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<Transaction>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }

    public async Task<PaginationResult<Transaction>> GetMyInternalTransactionsAsync(
        string userId,
        Func<IQueryable<Transaction>, IQueryable<Transaction>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.Transactions
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.CryptoToken)
            .Include(x => x.TransactionInternal)
            .Where(x => x.UserId == userId && x.SourceType == TransactionSourceType.Uxm)
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<Transaction>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }

    public async Task<PaginationResult<Transaction>> GetMyExternalTransactionsAsync(
        string userId,
        Func<IQueryable<Transaction>, IQueryable<Transaction>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.Transactions
            .AsNoTracking()
            .Include(x => x.User)
            .Include(x => x.CryptoToken)
            .Include(x => x.TransactionExternal)
                .ThenInclude(x => x!.GamePlatform)
            .Where(x => x.UserId == userId && x.SourceType != TransactionSourceType.Uxm && x.SourceType != TransactionSourceType.GameX)
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<Transaction>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }

    public async Task<PaginationResult<WalletTransactionDto>> GetMyWalletTransactionsAsync(
        string userId,
        Func<IQueryable<WalletTransactionDto>, IQueryable<WalletTransactionDto>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.Transactions
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ProjectToType<WalletTransactionDto>()
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<WalletTransactionDto>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }

    public async Task<bool> ExistsByOrderNoAsync(string otcOrderNo, CancellationToken ct)
    {
        return await context.Transactions
            .Include(x => x.TransactionInternal)
            .AnyAsync(x => x.TransactionInternal!.OrderNumber == otcOrderNo, ct);
    }

    public async Task<Transaction?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct)
    {
        return await context.Transactions
            .AsNoTracking()
            .Include(t => t.User)
            .ThenInclude(u => u.UserBalances)
            .Include(t => t.CryptoToken)
            .Include(t => t.TransactionInternal)
            .FirstOrDefaultAsync(x => x.TransactionInternal != null && x.TransactionInternal.OrderNumber == orderNumber, ct);
    }

    public async Task<Transaction> GetInternalByIdAsync(Guid publicId, CancellationToken ct = default)
    {
        return await context.Transactions
            .AsNoTracking()
            .Include(t => t.User)
                .ThenInclude(u => u.UserBalances)
            .Include(t => t.CryptoToken)
            .Include(x => x.TransactionInternal)
            .FirstOrDefaultAsync(x => x.PublicId == publicId, ct)
            ?? throw new NotFoundException(MessageCode.Transaction.TradeNotFound);
    }

    public async Task<Transaction> GetExternalByIdAsync(Guid publicId, CancellationToken ct = default)
    {
        return await context.Transactions
            .AsNoTracking()
            .Include(t => t.User)
            .ThenInclude(u => u.UserBalances)
            .Include(t => t.CryptoToken)
            .Include(x => x.TransactionExternal)
                .ThenInclude(x => x!.GamePlatform)
            .FirstOrDefaultAsync(x => x.PublicId == publicId, ct)
            ?? throw new NotFoundException(MessageCode.Transaction.TradeNotFound);
    }

    public async Task<Transaction> GetByIdAndUserIdAsync(string userId, Guid publicId, CancellationToken ct = default)
    {
        return await context.Transactions
            .AsNoTracking()
            .Include(t => t.User)
            .ThenInclude(u => u.UserBalances)
            .Include(t => t.CryptoToken)
            .Include(x => x.TransactionInternal)
            .Include(x => x.TransactionExternal)
                 .ThenInclude(x => x!.GamePlatform)
            .FirstOrDefaultAsync(x => x.PublicId == publicId && x.UserId == userId, ct)
            ?? throw new NotFoundException(MessageCode.Transaction.TradeNotFound);
    }

    public async Task<decimal> GetLatestBalanceAfterAsync(string userId, CancellationToken ct = default)
    {
        var tx = await context.Transactions
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.Status == TransactionStatus.Completed)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(ct);

        return tx?.BalanceAfter ?? 0;
    }

    public async Task<Transaction?> GetLatestExternalTransactionAsync(string userId, int localPlatformId, CancellationToken ct = default)
    {
        return await context.Transactions
            .AsNoTracking()
            .Include(tx => tx.TransactionExternal)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(x => x.UserId == userId
                && (x.Status == TransactionStatus.Completed)
                && (x.TransactionExternal != null)
                && (x.TransactionExternal.GamePlatformId == localPlatformId), ct);
    }

    public async Task AddAsync(Transaction transaction, CancellationToken ct = default)
    {
        await context.Transactions.AddAsync(transaction, ct);
    }

    public async Task UpdateAsync(Guid publicId, Action<Transaction> updateAction, CancellationToken ct = default)
    {
        var tx = await context.Transactions
            .Include(t => t.CryptoToken)
            .Include(t => t.TransactionInternal)
            .Include(t => t.TransactionExternal)
            .FirstOrDefaultAsync(c => c.PublicId == publicId, ct)
            ?? throw new NotFoundException(MessageCode.Transaction.ChainTransactionNotFound);

        updateAction.Invoke(tx);
    }
    public async Task UpdateAsync(Guid publicId, Func<Transaction, Task> updateAction, CancellationToken ct = default)
    {
        var tx = await context.Transactions
            .Include(t => t.CryptoToken)
            .Include(t => t.TransactionInternal)
            .Include(t => t.TransactionExternal)
            .FirstOrDefaultAsync(c => c.PublicId == publicId, ct)
            ?? throw new NotFoundException(MessageCode.Transaction.ChainTransactionNotFound);

        await updateAction.Invoke(tx);
    }
    public async Task UpdateAsync(Transaction transaction, CancellationToken ct = default)
    {
        context.Entry(transaction).State = EntityState.Modified;
        await context.SaveChangesAsync(ct);
    }
}