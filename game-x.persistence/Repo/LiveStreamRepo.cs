using EFCore.BulkExtensions;
using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.LiveStreams.Dtos;
using Mapster;

namespace game_x.persistence.Repo;

public sealed class LiveStreamRepo(GameXContext context) : ILiveStreamRepo, IRepository
{
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
            .ThenInclude(u => u != null ? u.Avatar : null)
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
            .ThenInclude(u => u != null ? u.Avatar : null)
            .FirstOrDefaultAsync(ls => ls.PublicId == id, ct)
            ?? throw new NotFoundException(nameof(id), id);
    }

    public async Task<LivestreamSchedule> GetDetailByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.LiveStreamSchedules
            .AsNoTracking()
            .Include(ls => ls.CategoryMappings)
            .ThenInclude(lsm => lsm.Category)
            .Include(ls => ls.AssignedTo)
            .ThenInclude(u => u != null ? u.Avatar : null)
            .Include(ls => ls.AssignedTo)
            .ThenInclude(u => u != null ? u.UserKyc : null)
            .Include(ls => ls.AssignedTo)
            .ThenInclude(u => u != null ? u.UserBankAccounts : null)
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

    public async Task BulkUpdateEndedStreams((Guid Id, DateTime EndTime)[] streamInfos, CancellationToken ct = default)
    {
        var scheduleIds = streamInfos.Select(si => si.Id);
        var targetSchedules = await context.LiveStreamSchedules
            .Where(ls => scheduleIds.Contains(ls.PublicId))
            .ToListAsync(ct);
        targetSchedules.ForEach(schedule =>
        {
            var (id, endTime) = streamInfos.FirstOrDefault(si => si.Id == schedule.PublicId);
            schedule.EndStream(endTime);
        });
        await context.BulkUpdateAsync(targetSchedules, cancellationToken: ct);
    }

    public async Task DeleteAsync(Guid scheduleId, CancellationToken ct = default)
    {
        var targetSchedule = await context.LiveStreamSchedules
            .FirstOrDefaultAsync(ls => ls.PublicId == scheduleId, ct)
            ?? throw new NotFoundException(nameof(scheduleId), scheduleId);

        context.LiveStreamSchedules.Remove(targetSchedule);
    }
}
