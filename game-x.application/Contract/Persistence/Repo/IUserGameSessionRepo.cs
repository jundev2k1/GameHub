using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.UserGameSessions.Dtos;

namespace game_x.application.Contract.Persistence.Repo;

public interface IUserGameSessionRepo
{
    Task<PaginationResult<UserGameSessionSearchItemDto>> GetsByCriteriaAsync(
        Func<IQueryable<UserGameSessionSearchItemDto>, IQueryable<UserGameSessionSearchItemDto>>? builder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<UserGameSession?> GetCurrentSessionByUserIdAsync(string userId, CancellationToken ct = default);

    Task CreateAsync(UserGameSession gameSession, CancellationToken ct = default);

    Task UpdateAsync(int id, Action<UserGameSession> updateAction, CancellationToken ct = default);

    Task AddConnectionAsync(UserGameSessionConnection connection, CancellationToken ct = default);

    Task UpdateConnectionAsync(long id, Action<UserGameSessionConnection> updateAction, CancellationToken ct = default);

    Task BulkUpdateExpiredGameSessionsAsync(int pageSize, CancellationToken ct = default);
}
