using game_x.application.Common.Abstractions.Pagination;
using System.Linq.Expressions;

namespace game_x.application.Contract.Persistence.Repo;

public interface ILiveStreamRepo
{
    Task<LivestreamSchedule[]> GetUnexpiredAsync(CancellationToken ct = default);

    Task<PaginationResult<LivestreamSchedule>> GetsByCriteriaAsync(
        Expression<Func<LivestreamSchedule, bool>> condition,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<LivestreamSchedule> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task CreateAsync(LivestreamSchedule schedule, CancellationToken ct = default);

    Task UpdateAsync(
        Guid scheduleId,
        Func<LivestreamSchedule, Task> updateAction,
        CancellationToken ct = default);

    Task DeleteAsync(Guid scheduleId, CancellationToken ct = default);
}
