using game_x.application.Features.OrderManagement.Dtos;
using game_x.domain.Identity;

namespace game_x.application.Features.Dashboard.Dtos;

public sealed class DashboardDto
{
    public int TotalMember { get; set; }
    public int RecentOrderCount { get; set; }
    public int PendingTransaction { get; set; }
    public decimal TotalRevenue { get; set; }
    public OrderDto[] Orders { get; set; } = Array.Empty<OrderDto>();
    public AppUser[] Members { get; set; } = Array.Empty<AppUser>();
}
