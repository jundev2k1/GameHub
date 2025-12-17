using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.UserGameSessions.Dtos;

namespace game_x.persistence.Repo;

public sealed class UserGameSessionRepo(GameXContext context) : IUserGameSessionRepo, IRepository
{
    public async Task<PaginationResult<UserGameSessionSearchItemDto>> GetsByCriteriaAsync(
        Func<IQueryable<UserGameSessionSearchItemDto>, IQueryable<UserGameSessionSearchItemDto>>? builder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.UserGameSessions
            .AsNoTracking()
            .Where(ugs => ugs.IsEnd)
            .Select(ugs => new UserGameSessionSearchItemDto
            {
                UserId = ugs.UserId,
                Nickname = ugs.User.Nickname,
                PlatformId = ugs.Platform.PublicId,
                GameCode = ugs.Game != null ? ugs.Game.GameCode : null,
                BalanceSnapshot = ugs.BalanceSnapshot,
                ConnectedAt = ugs.Connections.Min(c => c.ConnectedAt),
                DisconnectedAt = ugs.Connections.Max(c => c.DisconnectedAt),
            })
            .AsQueryable();

        if (builder is not null)
            query = builder(query);

        var totalItems = await query.CountAsync(ct);
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(ct);
        return new PaginationResult<UserGameSessionSearchItemDto>(
            items,
            totalItems,
            totalPages,
            page,
            pageSize);
    }

    public async Task<UserGameSession?> GetCurrentSessionByUserIdAsync(string userId, int platformId, CancellationToken ct = default)
    {
        var currentTime = DateTime.UtcNow;
        return await context.UserGameSessions
            .AsNoTracking()
            .Include(ugs => ugs.Connections)
            .FirstOrDefaultAsync(ugs =>
                ugs.UserId == userId
                && ugs.PlatformId == platformId
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

    public async Task UpdateConnectionAsync(long id, Action<UserGameSessionConnection> updateAction, CancellationToken ct = default)
    {
        var targetSession = await context.UserGameSessionConnections
            .FirstOrDefaultAsync(s => s.Id == id, ct)
            ?? throw new NotFoundException(nameof(id), id);

        updateAction?.Invoke(targetSession);
    }

    public async Task BulkUpdateExpiredGameSessionsAsync(int pageSize, CancellationToken ct = default)
    {
        var currentTime = DateTime.UtcNow.AddMinutes(5);
        var index = 0;

        var isContinues = true;
        while (isContinues)
        {
            var data = context.UserGameSessions
                .Where(ugs => !ugs.IsEnd && !ugs.Connections.All(c => c.DisconnectedAt != null && c.DisconnectedAt < currentTime))
                .Skip(pageSize * index)
                .Take(pageSize);
            if (!await data.AnyAsync(ct))
            {
                isContinues = false;
                break;
            }
            await data.ExecuteUpdateAsync(setters => setters.SetProperty(ugs => ugs.IsEnd, true), ct);
            index++;
        }
    }
}
