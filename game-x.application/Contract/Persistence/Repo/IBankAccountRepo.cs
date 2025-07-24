using game_x.application.Common.Abstractions.Pagination;

namespace game_x.application.Contract.Persistence.Repo;

public interface IBankAccountRepo
{
    Task<BankAccount[]> GetsByOwnerIdAsync(string ownerId, CancellationToken ct = default);

    Task<BankAccount> GetByIdAsync(int bankAccountId, CancellationToken ct = default);

    Task<BankAccount> GetByIdAsync(Guid bankAccountId, CancellationToken ct = default);

    Task<bool> IsExistsAsync(string ownerId, string accountNumber, CancellationToken ct = default);

    Task AddAsync(BankAccount bankAccount, CancellationToken ct = default);

    Task UpdateAsync(Guid bankAccountCode, string ownerId, Action<BankAccount> updateAction, CancellationToken ct = default);

    Task UpdateDefaultAccountAsync(Guid bankAccountId, string ownerId, CancellationToken ct = default);

    Task DeleteAsync(Guid bankAccountCode, string ownerId, CancellationToken ct = default);

    Task<PaginationResult<BankAccount>> GetBankAccountByCriteriaAsync(string userId,
       Func<IQueryable<BankAccount>, IQueryable<BankAccount>>? queryBuilder = null,
       int page = 1,
       int pageSize = 20,
       CancellationToken ct = default);

}
