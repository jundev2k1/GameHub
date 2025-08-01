using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.domain.Constants;

namespace game_x.persistence.Repo;

public sealed class ChainTransactionRepo(GameXContext context) : IChainTransactionRepo
{
    public IQueryable<ChainTransaction> Query()
    {
        return context.ChainTransactions;
    }

    public async Task<bool> ExistsAsync(string hash, CancellationToken ct)
    {
        return await context.ChainTransactions.AnyAsync(x => x.TransactionHash == hash, ct);
    }

    public async Task<bool> ExistsByOrderNoAsync(string otcOrderNo, CancellationToken ct)
    {
        return await context.ChainTransactions.AnyAsync(cl => cl.OrderNumber == otcOrderNo, ct);
    }

    public async Task<ChainTransaction?> GetByHashAsync(string hash, CancellationToken ct)
    {
        return await context.ChainTransactions.FirstOrDefaultAsync(x => x.TransactionHash == hash, ct);
    }

    public async Task<ChainTransaction?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct)
    {
        return await context.ChainTransactions.FirstOrDefaultAsync(x => x.OrderNumber == orderNumber, ct);
    }


    public async Task AddAsync(ChainTransaction chainTransaction, CancellationToken ct = default)
    {
        await context.ChainTransactions.AddAsync(chainTransaction, ct);
    }

    public async Task AddAsync(ChainTransaction chainTransaction, CancellationToken ct = default)
    {
        await context.ChainTransactions.AddAsync(chainTransaction, ct);
    }
    
    public async Task UpdateAsync(Guid chainTransactionId, Action<ChainTransaction> updateAction, CancellationToken ct = default)
    {
        var chainTransaction = await context.ChainTransactions
            .FirstOrDefaultAsync(c => c.PublicId == chainTransactionId, ct)
            ?? throw new NotFoundException(MessageCode.Transaction.ChainTransactionNotFound);

        updateAction.Invoke(chainTransaction);
    }
}