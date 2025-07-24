using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Jobs;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.infrastructure.BackgroundJobs.Jobs;

public sealed class DashboardStatisticJob(
    IDashboardCacheService dashboardCache,
    IOptions<RecurringJobSettings> jobOptions) : IRecurringJob
{
    public string JobId => "dashboard-statistic";
    public string CronExpression => jobOptions.Value.DashboardStatisticJob;
    public bool IsInit => true;

    public async Task ExecuteAsync(CancellationToken ct = default)
    {
        await dashboardCache.RefreshDataAsync(ct);
    }
}
