using game_x.application.Common.Abstractions.Pagination;

namespace game_x.application.Contract.Persistence.Repo;

public interface ITalentWalletRepo
{
    Task<PaginationResult<TalentWalletTransaction>> GetsByCriteriaAsync(
        Func<IQueryable<TalentWalletTransaction>, IQueryable<TalentWalletTransaction>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<TalentWallet> GetWalletAsync(string userId, CancellationToken ct = default);

    Task UpdateAsync(string userId, Action<TalentWallet> updateAction, CancellationToken ct = default);
}
