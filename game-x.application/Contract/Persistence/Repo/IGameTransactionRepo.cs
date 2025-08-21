using game_x.application.Common.Abstractions.Pagination;

namespace game_x.application.Contract.Persistence.Repo;

public interface IGameTransactionRepo
{
    Task<PaginationResult<GameTransaction>> GetMyTransactionsAsync(
        string userId,
        Func<IQueryable<GameTransaction>, IQueryable<GameTransaction>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);
    Task<GameTransaction> GetByIdAndUserIdAsync(string userId, Guid publicId, CancellationToken ct = default);
    Task<GameTransaction> AddAsync(GameTransaction entity, CancellationToken ct = default);
    /// <summary>Only update the fields that are passed in.</summary>
    Task PatchUpdateAsync(Guid publicId, Action<GameTransaction> updateAction, CancellationToken ct = default);
    /// <summary>Override all data of the record.</summary>
    Task PutUpdateAsync(GameTransaction transaction, CancellationToken ct = default);
}
