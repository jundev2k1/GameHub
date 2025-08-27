using game_x.application.Common.Abstractions.Pagination;

namespace game_x.application.Contract.Persistence.Repo;

public interface IGameTransactionRepo
{
    Task<PaginationResult<GameTransaction>> GetTransactionByCriteriaAsync(
        Func<IQueryable<GameTransaction>, IQueryable<GameTransaction>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<PaginationResult<GameTransaction>> GetMyTransactionsAsync(
        string userId,
        Func<IQueryable<GameTransaction>, IQueryable<GameTransaction>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<GameTransaction> GetByIdAsync(Guid publicId, CancellationToken ct = default);

    Task<GameTransaction> AddAsync(GameTransaction entity, CancellationToken ct = default);

    /// <summary>Only update the fields that are passed in.</summary>
    Task UpdateAsync(Guid publicId, Action<GameTransaction> updateAction, CancellationToken ct = default);
    /// <summary>Override all data of the record.</summary>
    Task UpdateAsync(GameTransaction transaction, CancellationToken ct = default);
}
