using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Features.Rewards.Processors;

namespace game_x.infrastructure.BackgroundJobs.Jobs;

public sealed class UserEventProcessingJob(
    IMissionRepo missionRepo,
    IUnitOfWork unitOfWork,
    IUserEventRepo userEventRepo,
    IMissionProcessor missionProcessor,
    IAppLogger<UserEventProcessingJob> logger
)
{
    public async Task ExecuteAsync(Guid userEventId, CancellationToken ct = default)
    {
        logger.LogInformation("Processing UserEvent {UserEventId}", userEventId);
        var userEvent = await userEventRepo.GetByIdAsync(userEventId, ct);
        if (userEvent is null)
        {
            logger.LogWarning("UserEvent {UserEventId} not found", userEventId);
            return;
        }
        
        var missionsTask = missionRepo.GetTriggeredByEventAsync(userEvent.Type, ct);
        await Task.WhenAll(missionsTask);
        
        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                foreach (var mission in missionsTask.Result)
                    await missionProcessor.ProcessAsync(mission, userEvent, ct);

                logger.LogInformation("Processed UserEvent {UserEventId}", userEventId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed processing UserEvent {UserEventId}", userEventId);
                throw;
            }
        }, ct);
    }
}