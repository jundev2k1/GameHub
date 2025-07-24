using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Features.Dashboard.Dtos;

namespace game_x.application.Features.Dashboard.Admin.Queries.GetDashboard;

public sealed class GetDashboardHandler(IDashboardCacheService dashboardCache)
    : IQueryHandler<GetDashboardQuery, GetDashboardResult>
{
    public async Task<GetDashboardResult> Handle(GetDashboardQuery request, CancellationToken ct = default)
    {
        var result = await Task.FromResult(dashboardCache.GetDashboard());
        return MapToResult(result);
    }

    private GetDashboardResult MapToResult(DashboardDto dto)
    {
        return new GetDashboardResult
        {
            TotalMembers = dto.TotalMember,
            PendingTransaction = dto.PendingTransaction,
            RecentOrderCount = dto.RecentOrderCount,
            TotalRevenue = dto.TotalRevenue,
            NewMembers = dto.Members.Select(m => new MemberInfo
            {
                MemberId = m.Id,
                MemberName = m.UserName ?? string.Empty,
                PassportNumber = m.Passport?.PassportNumber ?? string.Empty,
                IsActive = m.Status == AppUserStatus.Active,
                CreatedAt = m.CreatedAt,
            }).ToArray(),
            RecentOrders = dto.Orders.Select(o => new OrderInfo
            {
                OwnerName = o.OwnerName ?? string.Empty,
                Price = o.PricePerUnit * o.Quantity,
                OrderType = o.OrderType,
                Status = o.OrderStatus,
                CreatedAt = o.CreatedAt,
            }).ToArray()
        };
    }
}
