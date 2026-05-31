using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.Transactions.Dtos;
using game_x.domain.Constants;

namespace game_x.persistence.Repo;

public class TransactionRepo(GameXContext context) : ITransactionRepo, IRepository
{
    private readonly static TransactionSourceType[] TransactionInternalSourceTypes = [TransactionSourceType.Payment, TransactionSourceType.Refund];

    public async Task<PaginationResult<Transaction>> GetInternalTransactionsAsync(
        Func<IQueryable<Transaction>, IQueryable<Transaction>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.Transactions
            .AsNoTracking()
            .Where(x => 
                x.TransactionInternal != null 
                && (x.TransactionInternal.SourceType == TransactionSourceType.Payment 
                    || x.TransactionInternal.SourceType == TransactionSourceType.Refund))
            .Include(x => x.User)
            .Include(x => x.CryptoToken)
            .Include(x => x.TransactionInternal)
            .Include(x => x.ReviewedBy)
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
            .Include(x => x.TransactionExternal)
                .ThenInclude(x => x!.GamePlatform)
            .Where(x => x.TransactionExternal != null)
            .Include(x => x.User)
            .Include(x => x.CryptoToken)
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
            .Include(x => x.ReviewedBy)
            .Where(x => 
                x.UserId == userId 
                && x.TransactionInternal != null
                && TransactionInternalSourceTypes.Contains(x.TransactionInternal.SourceType))
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
            .Where(x => x.UserId == userId && x.TransactionExternal != null)
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
            .Select(tx => new WalletTransactionDto
            {
                Id = tx.PublicId,
                UserId = tx.UserId,
                Type = tx.Type,
                Status = tx.Status,
                Amount = tx.Amount,
                BalanceAfter = tx.BalanceAfter,
                ActualAmount = tx.ActualAmount ?? 0,
                GameAmount = tx.GameAmount,
                GameBalanceAfter = tx.GameBalanceAfter,
                Note = tx.Note,
                CryptoTokenId = tx.CryptoToken.PublicId,
                Symbol = tx.CryptoToken.Symbol,
                Network = tx.CryptoToken.Network,
                GamePlatformId = tx.TransactionExternal != null ? tx.TransactionExternal.GamePlatform.PublicId : null,
                GamePlatformName = tx.TransactionExternal != null ? tx.TransactionExternal.GamePlatform.Name : null,
                SourceType = tx.TransactionInternal != null ? tx.TransactionInternal.SourceType : null,
                Provider = tx.TransactionInternal != null ? tx.TransactionInternal.ProviderId : null,
                WalletSourceType = tx.TransactionInternal != null ? WalletSourceType.Internal : WalletSourceType.External,
                From = tx.TransactionInternal != null 
                    ? tx.TransactionInternal.SourceType == TransactionSourceType.Payment
                        ? tx.TransactionInternal.FromAddress
                        : tx.Type == TransactionType.TransferSent
                            ? tx.User.Nickname
                            : tx.TransactionInternal.Reference != null ? tx.TransactionInternal.Reference.User.Nickname : null
                    : null,
                To = tx.TransactionInternal != null 
                    ? tx.TransactionInternal.SourceType == TransactionSourceType.Payment
                        ? tx.TransactionInternal.ToAddress
                        : tx.Type == TransactionType.TransferReceived
                            ? tx.User.Nickname
                            : tx.TransactionInternal.Reference != null ? tx.TransactionInternal.Reference.User.Nickname : null
                    : null,
                CreatedAt = tx.CreatedAt,
                UpdatedAt = tx.UpdatedAt
            })
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
            .Include(x => x.ReviewedBy)
            .FirstOrDefaultAsync(x => x.TransactionInternal != null && x.TransactionInternal.OrderNumber == orderNumber, ct);
    }

    public async Task<Transaction> GetInternalByIdAsync(Guid publicId, CancellationToken ct = default)
    {
        return await context.Transactions
            .AsNoTracking()
            .AsSplitQuery()
            .Include(t => t.User)
                .ThenInclude(u => u.UserBalances)
            .Include(t => t.CryptoToken)
            .Include(x => x.TransactionInternal)
            .Include(x => x.ReviewedBy)
            .FirstOrDefaultAsync(x => x.PublicId == publicId, ct)
            ?? throw new NotFoundException(MessageCode.Transaction.TradeNotFound);
    }

    public async Task<TransactionTransferDto> GetTransferByIdAsync(Guid publicId, CancellationToken ct = default)
    {
        return await context.Transactions
            .AsNoTracking()
            .Where(x => 
                x.PublicId == publicId && 
                x.TransactionInternal != null && 
                x.TransactionInternal.SourceType == TransactionSourceType.Transfer)
            .Select(x => new TransactionTransferDto
            {
                Id = x.PublicId,
                Status = x.Status,
                Type = x.Type,
                Amount = x.Amount,
                BalanceAfter = x.BalanceAfter,
                Note = x.Note,
                SourceType = x.TransactionInternal!.SourceType,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                ReceiverId = x.TransactionInternal != null 
                    ? x.Type == TransactionType.TransferReceived
                        ? x.UserId
                        : x.TransactionInternal.Reference != null ? x.TransactionInternal.Reference.UserId : null
                    : null,
                TransferorId = x.TransactionInternal != null 
                    ? x.Type == TransactionType.TransferSent
                        ? x.UserId 
                        : x.TransactionInternal.Reference != null ? x.TransactionInternal.Reference.UserId : null
                    : null,
                From = x.TransactionInternal != null 
                    ? x.Type == TransactionType.TransferSent
                        ? x.User.Nickname
                        : x.TransactionInternal.Reference != null ? x.TransactionInternal.Reference.User.Nickname : String.Empty
                    : String.Empty,
                To = x.TransactionInternal != null 
                    ? x.Type == TransactionType.TransferReceived
                        ? x.User.Nickname
                        : x.TransactionInternal.Reference != null ? x.TransactionInternal.Reference.User.Nickname : String.Empty
                    : String.Empty,
                CryptoTokenId = x.CryptoToken.PublicId,
                Symbol = x.CryptoToken.Symbol,
                Network = x.CryptoToken.Network,
            })
            .FirstOrDefaultAsync(ct)
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

    public async Task<int> ExpiredTransactionAsync(int expireTimeSeconds, CancellationToken ct = default)
    {
        var threshold = DateTime.UtcNow.AddSeconds(-expireTimeSeconds);
        
        var expiredTransactions = await context.Transactions
            .Where(x => 
                x.Type == TransactionType.Deposit
                && x.Status == TransactionStatus.Pending
                && x.ExpiredAt == null
                && x.CreatedAt <= threshold)
            .ToListAsync(ct);
        
        foreach (var tx in expiredTransactions) { tx.Expired(); }
        return expiredTransactions.Count;
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