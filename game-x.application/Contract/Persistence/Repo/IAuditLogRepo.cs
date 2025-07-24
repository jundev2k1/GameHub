using game_x.application.Common.Abstractions.Pagination;

namespace game_x.application.Contract.Persistence.Repo;

public interface IAuditLogRepo
{
    Task<PaginationResult<AuditLog>> GetByCriteriaAsync(
        Func<IQueryable<AuditLog>, IQueryable<AuditLog>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<AuditLog> GetByIdAsync(Guid code, CancellationToken ct = default);

    Task AddAsync(AuditLog auditLog, CancellationToken ct = default);
}
