using game_x.application.Common.Abstractions.Pagination;

namespace game_x.application.Contract.Persistence.Repo;

public interface ISystemWalletRepo
{
    Task<PaginationResult<SystemWalletTransaction>> GetsByCriteriaAsync(
        Func<IQueryable<SystemWalletTransaction>, IQueryable<SystemWalletTransaction>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<SystemWallet[]> GetAllAsync(CancellationToken ct = default);

    Task<SystemWallet> GetWalletAsync(SystemWalletType type, CancellationToken ct = default);

    Task UpdateAsync(SystemWalletType type, Action<SystemWallet> updateAction, CancellationToken ct = default);
}
