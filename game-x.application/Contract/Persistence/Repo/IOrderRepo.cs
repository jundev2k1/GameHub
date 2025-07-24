using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.OrderManagement.Dtos;

namespace game_x.application.Contract.Persistence.Repo;

public interface IOrderRepo
{
    Task<OrderStatisticsDto> GetOrderStatisticsAsync(CancellationToken ct = default);

    Task<PaginationResult<Order>> GetByCriteriaAsync(
        Func<IQueryable<Order>, IQueryable<Order>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<PaginationResult<Order>> GetOrderCriteriaByClientAsync(
        string userId,
        Func<IQueryable<Order>, IQueryable<Order>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<PaginationResult<Order>> GetOrderOfUserByCriteriaAsync(
        string userId,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default);

    Task<Order?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);

    /// <summary>Used to create temporary data for display</summary>
    Task AddAsync(Order order, CancellationToken ct = default);

    Task UpdateAsync(int orderId, Action<Order> updateAction, CancellationToken ct = default);
    Task UpdateAsync(Guid orderPublicId, Action<Order> updateAction, CancellationToken ct = default);
    Task UpdateAsync(string orderUxmId, Action<Order> updateAction, CancellationToken ct = default);
}