using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.persistence.Repo;

public sealed class UserGameSessionRepo(GameXContext context) : IUserGameSessionRepo, IRepository
{
    public async Task<PaginationResult<UserGameSession>> GetsByCriteriaAsync(
        Func<IQueryable<UserGameSession>, IQueryable<UserGameSession>>? builder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.UserGameSessions
            .AsNoTracking()
            .AsQueryable();

        if (builder is not null)
            query = builder(query);

        var totalItems = await query.CountAsync(ct);
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(ct);
        return new PaginationResult<UserGameSession>(
            items,
            totalItems,
            totalPages,
            page,
            pageSize);
    }

    public async Task CreateAsync(UserGameSession gameSession, CancellationToken ct = default)
    {
        await context.UserGameSessions.AddAsync(gameSession, ct);
    }
}
