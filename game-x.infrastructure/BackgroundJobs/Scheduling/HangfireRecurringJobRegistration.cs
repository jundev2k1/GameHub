using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Jobs;
using game_x.share.Extensions;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace game_x.infrastructure.BackgroundJobs.Scheduling;

public static class HangfireRecurringJobRegistration
{
    public static void RegisterRecurringJobs(IServiceProvider serviceProvider)
    {
        var jobManager = serviceProvider.GetRequiredService<IRecurringJobManager>();
        var allJobs = serviceProvider.GetServices<IRecurringJob>();
        var logger = serviceProvider.GetRequiredService<IAppLogger<IRecurringJobManager>>();
        foreach (var job in allJobs)
        {
            // Register the job if it has a Cron expression
            if (job.CronExpression.IsNotNullOrEmpty())
            {
                jobManager.AddOrUpdate(
                    job.JobId,
                    () => job.ExecuteAsync(CancellationToken.None),
                    job.CronExpression);
                logger.LogInformation($"Registered recurring job: {job.JobId} with Cron: {job.CronExpression}");
            }

            // Execute the job immediately if it's marked as init
            if (job.IsInit)
            {
                BackgroundJob.Enqueue(() => job.ExecuteAsync(CancellationToken.None));
                logger.LogInformation($"Job run when application starts: {job.CronExpression}");
            }
        }
    }
}
