using game_x.application.Common.Abstractions.Pagination;
using System.Linq.Expressions;

namespace game_x.application.Contract.Persistence.Repo;

public interface IGameRepo
{
    Task<Game[]> GetAllAsync(CancellationToken ct = default);

    Task<PaginationResult<Game>> GetsByCriteriaAsync(
        Expression<Func<Game, bool>> condition,
        int Page,
        int PageSize,
        CancellationToken ct = default);

    Task UpdateGameAsync(
        Guid gameId,
        Func<Game, Task> updateAction,
        CancellationToken ct = default);
}
