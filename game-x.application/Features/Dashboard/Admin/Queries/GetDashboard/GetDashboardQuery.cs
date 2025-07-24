namespace game_x.application.Features.Dashboard.Admin.Queries.GetDashboard;

public record GetDashboardQuery : IQuery<GetDashboardResult>;

public sealed class GetDashboardResult
{
    public int TotalMembers { get; set; }
    public int RecentOrderCount { get; set; }
    public int PendingTransaction { get; set; }
    public decimal TotalRevenue { get; set; }
    public ICollection<OrderInfo> RecentOrders { get; set; } = Array.Empty<OrderInfo>();
    public ICollection<MemberInfo> NewMembers { get; set; } = Array.Empty<MemberInfo>();
}

public sealed class OrderInfo
{
    public string OwnerName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string OrderType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public sealed class MemberInfo
{
    public string MemberId { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string PassportNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
