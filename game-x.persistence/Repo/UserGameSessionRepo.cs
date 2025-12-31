using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.UserGameSessions.Dtos;

namespace game_x.persistence.Repo;

public sealed class UserGameSessionRepo(GameXContext context, IUnitOfWork unitOfWork) : IUserGameSessionRepo, IRepository
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
                Email = ugs.User.Email ?? string.Empty,
                PlatformId = ugs.Platform.PublicId,
                PlatformName = ugs.Platform.Name,
                GameId = ugs.Game != null ? ugs.Game.PublicId : null,
                GameCode = ugs.Game != null ? ugs.Game.GameCode : null,
                GameName = ugs.Game != null ? ugs.Game.Name : null,
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

    public async Task<UserGameSession?> GetCurrentSessionByUserIdAsync(string userId, int platformId, int? gameId, CancellationToken ct = default)
    {
        return await context.UserGameSessions
            .Include(ugs => ugs.Connections)
            .FirstOrDefaultAsync(ugs =>
                ugs.UserId == userId
                && ugs.PlatformId == platformId
                && ugs.GameId == gameId
                && !ugs.IsEnd, ct);
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

    public async Task RegisterConnectionAsync(UserGameSessionConnection connection, CancellationToken ct = default)
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

    public async Task CheckOutAsync(string connectionId, CancellationToken ct = default)
    {
        await context.UserGameSessionConnections
            .Where(uc => uc.ConnectionId == connectionId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.DisconnectedAt, DateTime.UtcNow), ct);
    }

    public async Task<bool> PingAsync(string connectionId, CancellationToken ct = default)
    {
        bool isSuccess = true;
        await unitOfWork.WithTransactionAsync(async () =>
        {
            var targetConnection = await context.UserGameSessionConnections
                .FirstOrDefaultAsync(usc => usc.ConnectionId == connectionId, ct);
            if (targetConnection == null)
            {
                isSuccess = false;
                return;
            }

            targetConnection.Ping();
        }, ct: ct);
        return isSuccess;
    }

    public async Task BulkUpdateExpiredGameSessionsAsync(CancellationToken ct = default)
    {
        var expiredAt = DateTime.UtcNow.AddMinutes(5);

        // Mark ended for User Sessions
        await context.UserGameSessions
            .Where(ugs =>
                !ugs.IsEnd
                && ugs.Connections.Any()
                && ugs.Connections.All(c => c.LastSeenAt < expiredAt))
            .ExecuteUpdateAsync(setters => setters.SetProperty(ugs => ugs.IsEnd, true), ct);

        // Set DisconnectedAt to LastSeenAt if records have been updated by clean up job
        await context.UserGameSessionConnections
            .Where(c => c.Session.IsEnd && c.DisconnectedAt == null)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    c => c.DisconnectedAt,
                    c => c.LastSeenAt),
                ct);
    }
}
