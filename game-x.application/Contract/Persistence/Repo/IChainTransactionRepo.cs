namespace game_x.application.Contract.Persistence.Repo;

public interface IChainTransactionRepo
{
    IQueryable<ChainTransaction> Query();
    Task<ChainTransaction?> GetByIdAsync(Guid publicId, CancellationToken ct = default);
    Task<bool> ExistsByOrderNoAsync(string otcOrderNo, CancellationToken ct);
    Task<ChainTransaction?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct);
    Task<(decimal txLogUserFrozenAmount, decimal chainTxLogpendingFee)> GetTxLogSummaryAsync(CancellationToken ct);
    Task AddAsync(ChainTransaction chainTransaction, CancellationToken ct = default);
    Task PatchUpdateAsync(Guid publicId, Action<ChainTransaction> updateAction, CancellationToken ct = default);
    Task PutUpdateAsync(ChainTransaction chain, CancellationToken ct = default);
}