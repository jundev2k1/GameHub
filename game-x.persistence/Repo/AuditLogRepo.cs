using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

namespace game_x.persistence.Repo;

public sealed class AuditLogRepo(GameXContext context) : IAuditLogRepo
{
    public async Task<PaginationResult<AuditLog>> GetByCriteriaAsync(Func<IQueryable<AuditLog>, IQueryable<AuditLog>>? queryBuilder = null, int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        var query = context.AuditLogs
            .AsNoTracking()
            .Include(o => o.ChangedBy)
            .AsQueryable();

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<AuditLog>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize
        );
    }

    public async Task<AuditLog> GetByIdAsync(Guid code, CancellationToken ct = default)
    {
        return await context.AuditLogs
            .FirstOrDefaultAsync(al => al.PublicId == code, ct)
            ?? throw new NotFoundException(nameof(AuditLog), nameof(code));
    }

    public async Task AddAsync(AuditLog auditLog, CancellationToken ct = default)
    {
        await context.AuditLogs.AddAsync(auditLog, ct);
    }
}