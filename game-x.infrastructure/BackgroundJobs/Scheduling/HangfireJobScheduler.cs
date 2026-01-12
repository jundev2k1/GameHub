using game_x.application.Common.Abstractions;
using game_x.application.Contract.Jobs;
using Hangfire;
using System.Linq.Expressions;

namespace game_x.infrastructure.BackgroundJobs.Scheduling;

public sealed class HangfireJobScheduler(IBackgroundJobClient client) : IJobScheduler, IServices
{
    public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay)
        => client.Schedule(methodCall, delay);
}
