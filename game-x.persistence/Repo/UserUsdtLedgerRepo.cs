using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.domain.Constants;

namespace game_x.persistence.Repo;

public sealed class UserUsdtLedgerRepo(GameXContext context) : IUserUsdtLedgerRepo, IRepository
{
    public IQueryable<UserUsdtLedger> Query()
    {
        return context.UserUsdtLedgers;
    }

    public async Task<PaginationResult<UserUsdtLedger>> GetUsdtLedgerUserByCriteriaAsync(
        string userId,
        Func<IQueryable<UserUsdtLedger>, IQueryable<UserUsdtLedger>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.UserUsdtLedgers
            .AsNoTracking()
            .Include(x => x.ChainTransaction)
            .ThenInclude(x => x!.CryptoToken)
            .Where(x => x.UserId == userId && x.FlowType != UsdtFlowType.Init)
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<UserUsdtLedger>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }

    public async Task<UserUsdtLedger?> GetLatestLedgerAsync(string userId)
    {
        return await context.UserUsdtLedgers
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<UserUsdtLedger> GetDetailByTransactionIdAsync(int transactionId)
    {
        return await context.UserUsdtLedgers
            .Where(x => x.ChainTransactionId == transactionId)
            .Include(x => x.ChainTransaction)
            .FirstOrDefaultAsync()
               ?? throw new NotFoundException(MessageCode.Transaction.ChainTransactionHistoryNotFound);
    }

    public async Task<UserUsdtLedger> GetDetailByUserAsync(string userId, Guid ledgerId, CancellationToken ct = default)
    {
        return await context.UserUsdtLedgers
            .AsNoTracking()
            .Include(x => x.ChainTransaction)
                .ThenInclude(x => x!.CryptoToken)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.PublicId == ledgerId, ct)
               ?? throw new NotFoundException(MessageCode.Transaction.ChainTransactionHistoryNotFound);
    }

    public async Task<UserUsdtLedger> GetDetailByIdAsync(Guid ledgerId, CancellationToken ct = default)
    {
        return await context.UserUsdtLedgers
            .AsNoTracking()
            .Include(x => x.ChainTransaction)
            .FirstOrDefaultAsync(x => x.PublicId == ledgerId, ct)
               ?? throw new NotFoundException(MessageCode.Transaction.ChainTransactionHistoryNotFound);
    }

    public async Task<UserUsdtLedger> GetDetailByIdAsync(int ledgerId, CancellationToken ct = default)
    {
        return await context.UserUsdtLedgers
            .AsNoTracking()
            .Include(x => x.ChainTransaction)
            .FirstOrDefaultAsync(x => x.Id == ledgerId, ct)
               ?? throw new NotFoundException(MessageCode.Transaction.ChainTransactionHistoryNotFound);
    }

    public async Task AddAsync(UserUsdtLedger userUsdtLedger, CancellationToken ct = default)
    {
        await context.UserUsdtLedgers.AddAsync(userUsdtLedger, ct);
        await context.SaveChangesAsync(ct);
    }
    public async Task<UserUsdtLedger> GetDetailByGameTransactionIdAsync(int transactionId)
    {
        return await context.UserUsdtLedgers
            .Where(x => x.GameTransactionId == transactionId)
            .Include(x => x.GameTransaction)
            .FirstOrDefaultAsync()
               ?? throw new NotFoundException(MessageCode.Transaction.ChainTransactionHistoryNotFound);
    }
}