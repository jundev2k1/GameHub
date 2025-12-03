using game_x.application.Common.Abstractions.Pagination;

namespace game_x.application.Contract.Persistence.Repo;

public interface IGameRepo
{
    Task<Game[]> GetAllAsync(CancellationToken ct = default);

    Task<Game> GetAsync(Guid gameId, CancellationToken ct = default);

    Task<PaginationResult<Game>> GetsByCriteriaAsync(
        Func<IQueryable<Game>, IQueryable<Game>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task UpdateGameAsync(
        Guid gameId,
        Func<Game, Task> updateAction,
        CancellationToken ct = default);

    Task DeleteAllCategoryMappingsAsync(Guid gameId, CancellationToken ct = default);

    Task DeleteAllTypeMappingsAsync(Guid gameId, CancellationToken ct = default);

    Task DeleteAllTagMappingsAsync(Guid gameId, CancellationToken ct = default);
}
