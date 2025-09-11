using game_x.application.Common.Abstractions.Pagination;
using System.Linq.Expressions;

namespace game_x.application.Contract.Persistence.Repo;

public interface ILiveStreamCategoryRepo
{
    Task<LiveStreamCategory[]> GetAllAsync(CancellationToken ct = default);

    Task<PaginationResult<LiveStreamCategory>> GetsByCriteriaAsync(
        Expression<Func<LiveStreamCategory, bool>> condition,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task CreateAsync(LiveStreamCategory category, CancellationToken ct = default);

    Task UpdateAsync(
        Guid categoryId,
        Func<LiveStreamCategory, Task> updateAction,
        CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
