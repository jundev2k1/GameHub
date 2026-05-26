using game_x.application.Contract.Infrastructure.BackgroundJobs.Dispatchers;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Features.Accounts.User.Commands.DailyCheckIn;
using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;
using Microsoft.Extensions.Logging;

namespace game_x.application.Events.Rewards.OnDailyCheckIn;

public sealed class OnDailyCheckInHandler(
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    IUserEventJobDispatcher userEventDispatcher,
    IUserEventRepo userEventRepo,
    ILogger<DailyCheckInHandler> logger) : IApplicationEventHandler<OnDailyCheckInEvent>
{
    public async Task Handle(OnDailyCheckInEvent @event, CancellationToken ct = default)
    {
        string userId = userAccessor.GetUserId();
        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                var id = Guid.CreateVersion7();
                var userEvent = UserEvent.Create(
                    userId: userId, 
                    type: UserEventType.DailyLogin, 
                    id: id);
                    
                await userEventRepo.AddAsync(userEvent, ct);
                await unitOfWork.CommitAsync(ct);
                userEventDispatcher.EnqueueProcess(id);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
            }
        }, ct);
    }
}