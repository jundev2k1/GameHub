using game_x.application.Common.Abstractions.Pagination;

namespace game_x.application.Contract.Persistence.Repo;

public interface IUserGameSessionRepo
{
    Task<PaginationResult<UserGameSession>> GetsByCriteriaAsync(
        Func<IQueryable<UserGameSession>, IQueryable<UserGameSession>>? builder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task CreateAsync(UserGameSession gameSession, CancellationToken ct = default);
}
