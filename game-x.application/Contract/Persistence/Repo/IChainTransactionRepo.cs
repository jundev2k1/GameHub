namespace game_x.application.Contract.Persistence.Repo;

public interface IChainTransactionRepo
{
    IQueryable<ChainTransaction> Query();
    Task<ChainTransaction?> GetByIdAsync(Guid publicId, CancellationToken ct = default);
    Task<bool> ExistsByOrderNoAsync(string otcOrderNo, CancellationToken ct);
    Task<ChainTransaction?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct);
    Task<(decimal txLogUserFrozenAmount, decimal chainTxLogpendingFee)> GetTxLogSummaryAsync(CancellationToken ct);
    Task AddAsync(ChainTransaction transaction, CancellationToken ct = default);
    /// <summary>Only update the fields that are passed in.</summary>
    Task PatchUpdateAsync(Guid publicId, Action<ChainTransaction> updateAction, CancellationToken ct = default);
    /// <summary>Override all data of the record.</summary>
    Task PutUpdateAsync(ChainTransaction transaction, CancellationToken ct = default);
}