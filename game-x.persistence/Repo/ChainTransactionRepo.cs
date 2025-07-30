

using game_x.application.Contract.Persistence.Repo;

namespace game_x.persistence.Repo;

public sealed class ChainTransactionRepo(GameXContext context) : IChainTransactionRepo
{
    public async Task AddAsync(ChainTransaction chainTransaction, CancellationToken ct = default)
    {
        await context.ChainTransactions.AddAsync(chainTransaction, ct);
    }

}
