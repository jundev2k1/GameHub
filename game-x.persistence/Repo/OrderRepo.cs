using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.OrderManagement.Dtos;

namespace game_x.persistence.Repo;

public sealed class OrderRepo(GameXContext context) : IOrderRepo
{
    public async Task<OrderStatisticsDto> GetOrderStatisticsAsync(CancellationToken ct = default)
    {
        var query = context.Orders
            .AsNoTracking()
            .AsQueryable();

        var orderByStatus = await query
            .GroupBy(order => order.OrderStatus)
            .ToDictionaryAsync(group => group.Key.Value, group => group.Count(), ct);
        var result = await query
            .GroupBy(_ => 1)
            .Select(group => new OrderStatisticsDto
            {
                TotalOrders = group.Count(),
                OrdersByStatus = orderByStatus
            })
            .FirstOrDefaultAsync(ct);
        return result!;
    }

    public async Task<PaginationResult<Order>> GetByCriteriaAsync(
        Func<IQueryable<Order>, IQueryable<Order>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.Counter)
            .Include(o => o.Staff)
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<Order>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }

    public async Task<PaginationResult<Order>> GetOrderCriteriaByClientAsync(
        string userId,
        Func<IQueryable<Order>, IQueryable<Order>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.Orders
            .AsNoTracking()
            .Include(o => o.Staff)
            .Include(o => o.Counter)
            .Where(o => o.UserId == userId)
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<Order>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }

    public async Task<PaginationResult<Order>> GetOrderOfUserByCriteriaAsync(
        string userId,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.Orders
            .AsNoTracking()
            .Include(o => o.Counter)
            .Where(o => o.UserId == userId);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<Order>(
            items,
            totalCount,
            (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }

    public async Task<Order?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
    {
        return await context.Orders
            .AsNoTracking()
            .Include(order => order.Counter)
            .FirstOrDefaultAsync(order => order.PublicId == orderId, ct);
    }

    public async Task AddAsync(Order order, CancellationToken ct = default)
    {
        await context.Orders.AddAsync(order, ct);
    }

    public async Task UpdateAsync(int orderId, Action<Order> updateAction, CancellationToken ct = default)
    {
        var targetOrder = await context.Orders
            .FirstOrDefaultAsync(order => order.Id == orderId, ct);
        if (targetOrder is null) throw new NotFoundException(nameof(Order), nameof(Order.Id));

        updateAction?.Invoke(targetOrder);
    }

    public async Task UpdateAsync(Guid orderPublicId, Action<Order> updateAction, CancellationToken ct = default)
    {
        var targetOrder = await context.Orders
            .FirstOrDefaultAsync(order => order.PublicId == orderPublicId, ct);
        if (targetOrder is null) throw new NotFoundException(nameof(Order), nameof(Order.PublicId));

        updateAction?.Invoke(targetOrder);
    }

    public async Task UpdateAsync(string orderUxmId, Action<Order> updateAction, CancellationToken ct = default)
    {
        var targetOrder = await context.Orders
            .FirstOrDefaultAsync(order => order.OrderUid == orderUxmId, ct)
            ?? throw new NotFoundException(nameof(orderUxmId), orderUxmId);

        updateAction?.Invoke(targetOrder);
    }
}
