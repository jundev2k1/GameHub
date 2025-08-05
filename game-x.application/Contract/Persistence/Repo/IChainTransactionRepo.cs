namespace game_x.application.Contract.Persistence.Repo;

public interface IChainTransactionRepo
{
    IQueryable<ChainTransaction> Query();
    Task<bool> ExistsAsync(string hash, CancellationToken ct);
    Task<bool> ExistsByOrderNoAsync(string otcOrderNo, CancellationToken ct);
    Task<ChainTransaction?> GetByHashAsync(string hash, CancellationToken ct);
    Task<ChainTransaction?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct);
    Task<(decimal txLogUserFrozenAmount, decimal chainTxLogpendingFee)> GetTxLogSummaryAsync(CancellationToken ct);
    Task AddAsync(ChainTransaction chainTransaction, CancellationToken ct = default);
    Task UpdateAsync(Guid chainTransactionId, Action<ChainTransaction> updateAction, CancellationToken ct = default);
}