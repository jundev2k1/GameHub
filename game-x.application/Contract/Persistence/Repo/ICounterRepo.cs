using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.CounterManagement.Dtos;

namespace game_x.application.Contract.Persistence.Repo;

public interface ICounterRepo
{
    Task<CounterStatisticsDto> GetCounterStatisticsAsync(Func<IQueryable<Order>, IQueryable<Order>>? queryBuilder = null, CancellationToken ct = default);

    Task<CounterStatisticsDto> GetCounterStatisticDetailAsync(Guid counterId, Func<IQueryable<Order>, IQueryable<Order>>? queryBuilder = null, CancellationToken ct = default);

    Task<PaginationResult<Counter>> GetByCriteriaAsync(
        Func<IQueryable<Counter>, IQueryable<Counter>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<Counter> GetByIdAsync(Guid counterCode, CancellationToken ct = default);

    Task<CounterNumber> CreateNewCounterNumberAsync(string staffId, CancellationToken ct = default);

    Task<bool> IsExistCounterAsync(Guid counterCode, CancellationToken ct = default);

    Task<bool> IsExistCounterNameAsync(string counterName, CancellationToken ct = default);

    Task<bool> IsExistNameExcludeIdAsync(string counterName, string excludedName, CancellationToken ct = default);

    Task AddAsync(Counter counter, CancellationToken ct = default);

    Task UpdateAsync(Guid counterCode, Action<Counter> updateAction, CancellationToken ct = default);

    Task DeleteAsync(Guid counterCode, CancellationToken ct = default);
}
