using game_x.application.Contract.Infrastructure.Caching;

namespace game_x.application.Events.OnDashboardRefresh;

public sealed class OnDashboardRefreshHandler(IDashboardCacheService dashboardCache)
    : IApplicationEventHandler<OnDashboardRefreshEvent>
{
    public async Task Handle(OnDashboardRefreshEvent @event, CancellationToken ct = default)
    {
        await dashboardCache.RefreshDataAsync(ct);
    }
}
