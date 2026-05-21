namespace game_x.application.Contract.Infrastructure.BackgroundJobs.Dispatchers;

public interface IUserEventJobDispatcher
{
    void EnqueueProcess(Guid userEventId);
}