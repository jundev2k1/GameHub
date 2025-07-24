using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Dashboard.Dtos;
using game_x.application.Features.OrderManagement.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching;

public sealed class DashboardCacheService(
    IMemoryCache cache,
    GameXContext context,
    IUserRepo userRepo) : CacheService(cache), IDashboardCacheService
{
    private const string TotalMemberCacheKey = "dashboard:total-new-member:now";
    private const string OrderCountCacheKey = "dashboard:order-count:now";
    private const string PendingTransactionCacheKey = "dashboard:pending-transaction:now";
    private const string TotalRevenueCacheKey = "dashboard:total-revenue:now";
    private const string CurrentOrdersCacheKey = "dashboard:current-orders:now";
    private const string NewMembersCacheKey = "dashboard:new-members:now";

    public DashboardDto GetDashboard()
    {
        var totalMember = Get<int>(TotalMemberCacheKey);
        var orderCount = Get<int>(OrderCountCacheKey);
        var pendingTransaction = Get<int>(PendingTransactionCacheKey);
        var totalRevenue = Get<decimal>(TotalRevenueCacheKey);
        var currentOrders = Get<OrderDto[]>(CurrentOrdersCacheKey);
        var newMembers = Get<AppUser[]>(NewMembersCacheKey);

        return new DashboardDto
        {
            TotalMember = totalMember,
            RecentOrderCount = orderCount,
            PendingTransaction = pendingTransaction,
            TotalRevenue = totalRevenue,
            Orders = currentOrders ?? [],
            Members = newMembers ?? []
        };
    }

    public async Task RefreshDataAsync(CancellationToken ct = default)
    {
        await SetStatistic(ct);
        await SetNewMembers(ct);
        await SetRecentOrders(ct);
    }

    private async Task SetStatistic(CancellationToken ct = default)
    {
        var totalMember = await GetTotalMember(ct);
        Set(TotalMemberCacheKey, totalMember);

        var recentOrderCount = await GetRecentOrderCount(ct);
        Set(OrderCountCacheKey, recentOrderCount);

        var pendingTransaction = await GetMatchingTransactionCount(ct);
        Set(PendingTransactionCacheKey, pendingTransaction);

        var totalRevenue = await GetTotalRevenue(ct);
        Set(TotalRevenueCacheKey, totalRevenue);
    }

    private async Task SetNewMembers(CancellationToken ct = default)
    {
        var limit = 5;
        var users = await context.Users
            .AsNoTracking()
            .Include(u => u.Passport)
            .Where(u => u.UserRoles.Any(role => role.Role.Name == AppRoles.User))
            .OrderByDescending(u => u.CreatedAt)
            .Take(limit)
            .ToArrayAsync(ct);
        Set(NewMembersCacheKey, users);
    }

    private async Task SetRecentOrders(CancellationToken ct = default)
    {
        var limit = 5;
        var result = await context.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .Where(o => o.User.UserRoles.Any(ur => ur.Role.Name == AppRoles.User))
            .OrderByDescending(o => o.CreatedAt)
            .Take(limit)
            .Select(o => o.Adapt<OrderDto>())
            .ToArrayAsync(ct);
        Set(CurrentOrdersCacheKey, result);
    }

    private async Task<int> GetTotalMember(CancellationToken ct = default)
    {
        var users = await userRepo.GetUserByRole(AppRoles.User);
        return users.Count(u => !u.IsDeleted);
    }

    private async Task<int> GetRecentOrderCount(CancellationToken ct = default)
    {
        return await context.Orders.CountAsync(ct);
    }

    private async Task<int> GetMatchingTransactionCount(CancellationToken ct = default)
    {
        return await context.Orders
            .CountAsync(o => o.OrderStatus == OrderStatus.Created, ct);
    }

    private async Task<decimal> GetTotalRevenue(CancellationToken ct = default)
    {
        return await context.Orders
            .Where(o => o.OrderStatus == OrderStatus.Approved)
            .SumAsync(o => o.TotalPrice, ct);
    }
}