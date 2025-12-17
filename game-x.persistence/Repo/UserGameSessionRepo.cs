using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;

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

    public async Task<UserGameSession?> GetCurrentSessionByUserIdAsync(string userId, CancellationToken ct = default)
    {
        var currentTime = DateTime.UtcNow;
        return await context.UserGameSessions
            .AsNoTracking()
            .Include(ugs => ugs.Connections)
            .FirstOrDefaultAsync(ugs =>
                ugs.UserId == userId
                && !ugs.IsEnd
                && ugs.Connections.Any(c => c.ConnectedAt < currentTime), ct);
    }

    public async Task CreateAsync(UserGameSession gameSession, CancellationToken ct = default)
    {
        await context.UserGameSessions.AddAsync(gameSession, ct);
    }

    public async Task UpdateAsync(int id, Action<UserGameSession> updateAction, CancellationToken ct = default)
    {
        var targetSession = await context.UserGameSessions
            .FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        updateAction?.Invoke(targetSession);
    }

    public async Task AddConnectionAsync(UserGameSessionConnection connection, CancellationToken ct = default)
    {
        await context.UserGameSessionConnections.AddAsync(connection, ct);
    }

    public async Task UpdateConnectionAsync(int id, Action<UserGameSessionConnection> updateAction, CancellationToken ct = default)
    {
        var targetSession = await context.UserGameSessionConnections
            .FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        updateAction?.Invoke(targetSession);
    }
}
