using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.domain.Constants;

namespace game_x.persistence.Repo;

public sealed class ChainTransactionRepo(GameXContext context): IChainTransactionRepo, IRepository
{
    public IQueryable<ChainTransaction> Query()
    {
        return context.ChainTransactions;
    }

    public async Task<ChainTransaction?> GetByIdAsync(Guid publicId, CancellationToken ct = default)
    {
        return await context.ChainTransactions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PublicId == publicId, ct);
    }
    
    public async Task<bool> ExistsByOrderNoAsync(string otcOrderNo, CancellationToken ct)
    {
        return await context.ChainTransactions.AnyAsync(cl => cl.OrderNumber == otcOrderNo, ct);
    }

    public async Task<ChainTransaction?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct)
    {
        return await context.ChainTransactions.FirstOrDefaultAsync(x => x.OrderNumber == orderNumber, ct);
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

    public async Task AddAsync(ChainTransaction chainTransaction, CancellationToken ct = default)
    {
        await context.ChainTransactions.AddAsync(chainTransaction, ct);
    }
    
    public async Task PatchUpdateAsync(Guid publicId, Action<ChainTransaction> updateAction, CancellationToken ct = default)
    {
        var chainTransaction = await context.ChainTransactions
            .FirstOrDefaultAsync(c => c.PublicId == publicId, ct)
            ?? throw new NotFoundException(MessageCode.Transaction.ChainTransactionNotFound);

        updateAction.Invoke(chainTransaction);
    }
    
    public async Task PutUpdateAsync(ChainTransaction chain, CancellationToken ct = default)
    {
        context.Entry(chain).State = EntityState.Modified;
        await context.SaveChangesAsync(ct);
    }
}