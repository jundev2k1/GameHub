using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.CounterManagement.Dtos;
using game_x.domain.Constants;

namespace game_x.persistence.Repo;

public sealed class CounterRepo(GameXContext context) : ICounterRepo
{
    public async Task<CounterStatisticsDto> GetCounterStatisticsAsync(
        Func<IQueryable<Order>, IQueryable<Order>>? queryBuilder = null,
        CancellationToken ct = default)
    {
        var query = context.Orders
            .AsNoTracking()
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalOrdersCount = await query.CountAsync(ct);
        var buyOrderQuery = query.Where(sc => sc.OrderType == OrderType.Buy);
        var totalBuyOrders = await buyOrderQuery.CountAsync(ct);
        var buyFiatAmount = await buyOrderQuery.SumAsync(o => o.FiatAmount, ct);
        var buyCryptoAmount = await buyOrderQuery.SumAsync(o => o.CryptoAmount, ct);
        var totalUxmBuyFee = await buyOrderQuery.SumAsync(o => o.Fee, ct);

        var sellOrderQuery = query.Where(sc => sc.OrderType == OrderType.Sell);
        var totalSellOrders = await sellOrderQuery.CountAsync(ct);
        var sellFiatAmount = await sellOrderQuery.SumAsync(o => o.FiatAmount, ct);
        var sellCryptoAmount = await sellOrderQuery.SumAsync(o => o.CryptoAmount, ct);
        var totalUxmSellFee = await sellOrderQuery.SumAsync(o => o.Fee, ct);

        return new CounterStatisticsDto
        {
            TotalOrders = totalOrdersCount,
            TotalBuyOrders = totalBuyOrders,
            TotalSellOrders = totalSellOrders,
            BuyFiatAmount = buyFiatAmount,
            BuyCryptoAmount = buyCryptoAmount,
            SellFiatAmount = sellFiatAmount,
            SellCryptoAmount = sellCryptoAmount,
            Profit = 0,
            TotalUxmBuyFee = totalUxmBuyFee,
            TotalUxmSellFee = totalUxmSellFee,
        };
    }

    public async Task<CounterStatisticsDto> GetCounterStatisticDetailAsync(
        Guid counterId,
        Func<IQueryable<Order>, IQueryable<Order>>? queryBuilder = null,
        CancellationToken ct = default)
    {
        var query = context.Orders
            .AsNoTracking()
            .Where(o => o.Counter.PublicId == counterId)
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalOrdersCount = await query.CountAsync(ct);

        var buyOrderQuery = query.Where(sc => sc.OrderType == OrderType.Buy);
        var totalBuyOrders = await buyOrderQuery.CountAsync(ct);
        var buyFiatAmount = await buyOrderQuery.SumAsync(o => o.FiatAmount, ct);
        var buyCryptoAmount = await buyOrderQuery.SumAsync(o => o.CryptoAmount, ct);
        var totalUxmBuyFee = await buyOrderQuery.SumAsync(o => o.Fee, ct);

        var sellOrderQuery = query.Where(sc => sc.OrderType == OrderType.Sell);
        var totalSellOrders = await sellOrderQuery.CountAsync(ct);
        var sellFiatAmount = await sellOrderQuery.SumAsync(o => o.FiatAmount, ct);
        var sellCryptoAmount = await sellOrderQuery.SumAsync(o => o.CryptoAmount, ct);
        var totalUxmSellFee = await sellOrderQuery.SumAsync(o => o.Fee, ct);

        return new CounterStatisticsDto
        {
            TotalOrders = totalOrdersCount,
            TotalBuyOrders = totalBuyOrders,
            TotalSellOrders = totalSellOrders,
            BuyFiatAmount = buyFiatAmount,
            BuyCryptoAmount = buyCryptoAmount,
            SellFiatAmount = sellFiatAmount,
            SellCryptoAmount = sellCryptoAmount,
            Profit = 0,
            TotalUxmBuyFee = totalUxmBuyFee,
            TotalUxmSellFee = totalUxmSellFee,
        };
    }

    public async Task<PaginationResult<Counter>> GetByCriteriaAsync(
        Func<IQueryable<Counter>, IQueryable<Counter>>? queryBuilder = null,
        int page = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = context.Counters
            .AsNoTracking()
            .Where(c => c.IsDeleted == false)
            .Include(c => c.CounterToken)
            .AsQueryable();

        if (queryBuilder != null)
            query = queryBuilder(query);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginationResult<Counter>(
            items,
            totalCount,
            totalPages: (int)Math.Ceiling((decimal)totalCount / pageSize),
            page,
            pageSize);
    }

    public async Task<bool> IsExistCounterNameAsync(string counterName, CancellationToken ct = default)
    {
        return await context.Counters
            .AnyAsync(c => c.CounterName == counterName && !c.IsDeleted, ct);
    }

    public async Task<Counter> GetByIdAsync(Guid counterCode, CancellationToken ct = default)
    {
        return await context.Counters
            .Include(counter => counter.CounterToken)
            .FirstOrDefaultAsync(counter => counter.PublicId == counterCode && !counter.IsDeleted, ct)
            ?? throw new NotFoundException(MessageCode.Counter.CounterNotFound);
    }

    public async Task<CounterNumber> CreateNewCounterNumberAsync(string staffId, CancellationToken ct = default)
    {
        var counterByUsers = await context.StaffCounters
            .Select(sc => sc.Counter.CounterNumber)
            .ToArrayAsync(ct);
        var maxCount = counterByUsers.Any()
            ? counterByUsers.Max(counterNumber => int.Parse(counterNumber.Value))
            : 0;
        var newId = (maxCount + 1).ToString().PadLeft(4, '0');
        return CounterNumber.Of(newId);
    }

    public async Task<bool> IsExistCounterAsync(Guid counterCode, CancellationToken ct = default)
    {
        return await context.Counters
            .AnyAsync(c => (c.PublicId == counterCode) && !c.IsDeleted, ct);
    }

    public async Task<bool> IsExistNameExcludeIdAsync(
        string counterName,
        string? currentName,
        CancellationToken ct = default)
    {
        return await context.Counters.AnyAsync(
            c => (c.CounterName == counterName) && !c.IsDeleted && (currentName != c.CounterName), ct);
    }

    public async Task AddAsync(Counter counter, CancellationToken ct = default)
    {
        await context.Counters.AddAsync(counter, ct);
    }

    public async Task UpdateAsync(Guid counterCode, Action<Counter> updateAction, CancellationToken ct = default)
    {
        var targetCounter = await context.Counters
            .FirstOrDefaultAsync(c => c.PublicId == counterCode, ct)
            ?? throw new NotFoundException(MessageCode.Counter.CounterNotFound);

        updateAction?.Invoke(targetCounter);
    }

    public async Task DeleteAsync(Guid counterCode, CancellationToken ct = default)
    {
        var targetCounter = await context.Counters
            .FirstOrDefaultAsync(c => c.PublicId == counterCode, ct)
            ?? throw new NotFoundException(MessageCode.Counter.CounterNotFound);

        context.Counters.Remove(targetCounter);
    }
}
