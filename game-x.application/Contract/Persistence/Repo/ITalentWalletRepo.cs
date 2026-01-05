using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.TalentWallets.DTOs;

namespace game_x.application.Contract.Persistence.Repo;

public interface ITalentWalletRepo
{
    Task<PaginationResult<TalentWalletTransactionDto>> GetsByCriteriaAsync(
        string? talentId = null,
        Func<IQueryable<TalentWalletTransactionDto>, IQueryable<TalentWalletTransactionDto>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<TalentWallet> GetWalletAsync(string userId, CancellationToken ct = default);

    Task UpdateAsync(string userId, Action<TalentWallet> updateAction, CancellationToken ct = default);
}
