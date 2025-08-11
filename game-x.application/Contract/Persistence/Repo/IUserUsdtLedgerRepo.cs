using game_x.application.Common.Abstractions.Pagination;

namespace game_x.application.Contract.Persistence.Repo;

public interface IUserUsdtLedgerRepo
{
    IQueryable<UserUsdtLedger> Query();

    Task<PaginationResult<UserUsdtLedger>> GetUsdtLedgerUserByCriteriaAsync(
        string userId,
        Func<IQueryable<UserUsdtLedger>, IQueryable<UserUsdtLedger>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);
    Task<UserUsdtLedger> GetLatestLedgerAsync(string userId);
    Task<UserUsdtLedger> GetDetailByUserAsync(string userId, Guid ledgerId, CancellationToken ct = default);
    Task<UserUsdtLedger> GetDetailByIdAsync(Guid ledgerId, CancellationToken ct = default);
    Task<UserUsdtLedger> GetDetailByIdAsync(int ledgerId, CancellationToken ct = default);
    Task<UserUsdtLedger> GetDetailByTransactionIdAsync(int transactionId);
    Task AddAsync(UserUsdtLedger userUsdtLedger, CancellationToken ct = default);
}