using game_x.application.Contract.Infrastructure.BackgroundJobs.Dispatchers;
using game_x.infrastructure.BackgroundJobs.Jobs;
using Hangfire;

namespace game_x.infrastructure.BackgroundJobs.Dispatchers;

public sealed class UserEventJobDispatcher(IBackgroundJobClient jobClient) : IUserEventJobDispatcher
{
    public void EnqueueProcess(Guid userEventId)
    {
        jobClient.Enqueue<UserEventProcessingJob>(
            x => x.ExecuteAsync(
                userEventId,
                CancellationToken.None));
    }
}