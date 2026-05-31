using game_x.application.Common.Abstractions.Pagination;

namespace game_x.application.Contract.Persistence.Repo;

public interface ILiveStreamCategoryRepo
{
    Task<LiveStreamCategory[]> GetAllAsync(CancellationToken ct = default);

    Task<PaginationResult<LiveStreamCategory>> GetsByCriteriaAsync(
        Func<IQueryable<LiveStreamCategory>, IQueryable<LiveStreamCategory>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<LiveStreamCategory[]> GetByIdsAsync(Guid[] ids, CancellationToken ct = default);

    Task<LiveStreamCategory> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task CreateAsync(LiveStreamCategory category, CancellationToken ct = default);

    Task UpdateAsync(
        Guid categoryId,
        Func<LiveStreamCategory, Task> updateAction,
        CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
