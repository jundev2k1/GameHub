using game_x.application.Common.Abstractions.Pagination;

namespace game_x.application.Contract.Persistence.Repo;

public interface ILiveStreamRepo
{
    Task<PaginationResult<LivestreamSchedule>> GetsByCriteriaAsync(
        Func<IQueryable<LivestreamSchedule>, IQueryable<LivestreamSchedule>>? builder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<LivestreamSchedule[]> GetExpiredStreams(CancellationToken ct = default);

    Task<LivestreamSchedule> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<LivestreamSchedule> GetDetailByIdAsync(Guid id, CancellationToken ct = default);

    Task<LivestreamSchedule> GetByStreamKeyAsync(string streamKey, CancellationToken ct = default);

    Task CreateAsync(LivestreamSchedule schedule, CancellationToken ct = default);

    Task UpdateAsync(
        Guid scheduleId,
        Func<LivestreamSchedule, Task> updateAction,
        CancellationToken ct = default);

    Task BulkUpdateEndedStreams(Guid[] streamIds, CancellationToken ct = default);

    Task DeleteAsync(Guid scheduleId, CancellationToken ct = default);
}
