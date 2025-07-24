using game_x.application.Contract.Jobs;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace game_x.infrastructure.BackgroundJobs.Scheduling;

public static class HangfireRecurringJobRegistration
{
    public static void RegisterRecurringJobs(IServiceProvider serviceProvider)
    {
        var jobManager = serviceProvider.GetRequiredService<IRecurringJobManager>();
        var allJobs = serviceProvider.GetServices<IRecurringJob>();
        foreach (var job in allJobs)
        {
            jobManager.AddOrUpdate(
                job.JobId,
                () => job.ExecuteAsync(CancellationToken.None),
                job.CronExpression);

            if (job.IsInit)
                BackgroundJob.Enqueue(() => job.ExecuteAsync(CancellationToken.None));
        }
    }
}
