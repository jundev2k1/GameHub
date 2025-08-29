using game_x.application.Common.Abstractions.Pagination;

namespace game_x.application.Contract.Persistence.Repo;

public interface ITransactionRepo
{
    Task<PaginationResult<Transaction>> GetTransactionByCriteriaAsync(
        Func<IQueryable<Transaction>, IQueryable<Transaction>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);
    Task<PaginationResult<Transaction>> GetMyInternalTransactionsAsync(
        string userId,
        Func<IQueryable<Transaction>, IQueryable<Transaction>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);
    Task<bool> ExistsByOrderNoAsync(string otcOrderNo, CancellationToken ct);
    Task<Transaction?> GetByOrderNumberAsync(string orderNumber, CancellationToken ct);
    Task<Transaction> GetByIdAsync(Guid publicId, CancellationToken ct = default);
    Task<Transaction> GetExternalByIdAsync(Guid publicId, CancellationToken ct = default);
    Task<Transaction> GetByIdAndUserIdAsync(string userId, Guid publicId, CancellationToken ct = default);
    /// <summary>
    /// Get the balance after from the lasted successful transaction of the user
    /// </summary>
    Task<decimal> GetLatestBalanceAfterAsync(string userId, CancellationToken ct = default);
    Task AddAsync(Transaction transaction, CancellationToken ct = default);
    /// <summary>Only update the fields that are passed in.</summary>
    Task PatchUpdateAsync(Guid publicId, Action<Transaction> updateAction, CancellationToken ct = default);
    Task PatchUpdateAsync(int id, Action<Transaction> updateAction, CancellationToken ct = default);
    /// <summary>Override all data of the record.</summary>
    Task PutUpdateAsync(Transaction transaction, CancellationToken ct = default);
}