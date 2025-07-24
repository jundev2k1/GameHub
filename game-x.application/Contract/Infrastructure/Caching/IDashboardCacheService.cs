using game_x.application.Features.Dashboard.Dtos;

namespace game_x.application.Contract.Infrastructure.Caching;

public interface IDashboardCacheService
{
    DashboardDto GetDashboard();

    Task RefreshDataAsync(CancellationToken ct = default);
}
