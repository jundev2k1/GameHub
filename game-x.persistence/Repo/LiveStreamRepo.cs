using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class LiveStreamRepo(GameXContext context) : ILiveStreamRepo, IRepository
{
    public async Task<LivestreamSchedule[]> GetUnexpiredAsync(CancellationToken ct = default)
    {
        var result = await context.LiveStreamSchedules
            .AsNoTracking()
            .Include(ls => ls.CategoryMappings)
            .ThenInclude(lsm => lsm.Category)
            .Include(ls => ls.AssignedTo)
            .Where(ls => ls.StartAt >= DateTime.UtcNow)
            .ToArrayAsync(ct);
        return result;
    }

    public async Task<PaginationResult<LivestreamSchedule>> GetsByCriteriaAsync(
        Func<IQueryable<LivestreamSchedule>, IQueryable<LivestreamSchedule>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.LiveStreamSchedules
            .AsNoTracking()
            .Include(g => g.CategoryMappings)
            .ThenInclude(lsm => lsm.Category)
            .Include(g => g.AssignedTo)
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var totalPageCount = (int)Math.Ceiling((decimal)totalCount / pageSize);
        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(ct);

        return new PaginationResult<LivestreamSchedule>(
            result,
            totalCount,
            totalPageCount,
            page,
            pageSize);
    }

    public async Task<LivestreamSchedule> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.LiveStreamSchedules
            .AsNoTracking()
            .Include(ls => ls.CategoryMappings)
            .ThenInclude(lsm => lsm.Category)
            .Include(ls => ls.AssignedTo)
            .FirstOrDefaultAsync(ls => ls.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);
    }

    public async Task<LivestreamSchedule> GetByStreamKeyAsync(string streamKey, CancellationToken ct = default)
    {
        return await context.LiveStreamSchedules
            .AsNoTracking()
            .Include(ls => ls.CategoryMappings)
            .ThenInclude(lsm => lsm.Category)
            .Include(ls => ls.AssignedTo)
            .FirstOrDefaultAsync(ls => ls.StreamKey == streamKey, ct)
            ?? throw new NotFoundException(nameof(streamKey), streamKey);
    }

    public async Task CreateAsync(LivestreamSchedule schedule, CancellationToken ct = default)
    {
        await context.LiveStreamSchedules.AddAsync(schedule, ct);
    }

    public async Task UpdateAsync(Guid scheduleId, Func<LivestreamSchedule, Task> updateAction, CancellationToken ct = default)
    {
        var targetSchedule = await context.LiveStreamSchedules
            .Include(ls => ls.CategoryMappings)
            .ThenInclude(lsm => lsm.Category)
            .Include(ls => ls.AssignedTo)
            .FirstOrDefaultAsync(ls => ls.PublicId == scheduleId, ct)
            ?? throw new NotFoundException(nameof(scheduleId), scheduleId);

        await updateAction.Invoke(targetSchedule);
    }

    public async Task DeleteAsync(Guid scheduleId, CancellationToken ct = default)
    {
        var targetSchedule = await context.LiveStreamSchedules
            .FirstOrDefaultAsync(ls => ls.PublicId == scheduleId, ct)
            ?? throw new NotFoundException(nameof(scheduleId), scheduleId);

        context.LiveStreamSchedules.Remove(targetSchedule);
    }
}
